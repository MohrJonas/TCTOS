using System.Diagnostics;
using TCTOS.Abstractions;
using TCTOS.Common;

namespace TCTOS.Impls.Local;

public sealed class AnsibleContainerProvisionerImpl : IContainerProvisioner
{
    public Task<Result> ProvisionContainer(string containerName, string provisionFileContent,
        Dictionary<string, string> variables)
    {
        return RunCatchingAsync(async () =>
        {
            List<string> args =
            [
                "-c",
                "community.general.incus",
                "-i",
                $"{containerName},"
            ];
            foreach (var (key, value) in variables)
            {
                args.Add("-e");
                args.Add($"{key}={value}");
            }

            args.Add("/dev/stdin");
            var startInfo = new ProcessStartInfo
            {
                FileName = "ansible-playbook",
                Arguments = string.Join(" ", args),
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            var process = Process.Start(startInfo);

            await process!.StandardInput.WriteAsync(provisionFileContent);
            await process.StandardInput.FlushAsync();
            process.StandardInput.Close();

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
                throw new Exception(
                    $"{await process.StandardError.ReadToEndAsync()}\n{await process.StandardOutput.ReadToEndAsync()}");
        });
    }

    public string GetDefaultProvisionFileTemplate()
    {
        return """
               - name: Container setup
                 hosts: all
                 tasks:
                   - name: Ensure python3 is installed for further Ansible operations
                     ansible.builtin.raw: |
                       if ! command -v python3 >/dev/null 2>&1; then
                         . /etc/os-release
                         case "$ID" in
                           ubuntu|debian)
                             apt-get update -y && apt-get install -y python3
                             ;;
                           fedora)
                             dnf install -y python3
                             ;;
                           almalinux|centos|rhel)
                             yum install -y python3
                             ;;
                           alpine)
                             apk update && apk add python3
                             ;;
                           *)
                             echo "Unsupported OS: $ID"
                             exit 1
                             ;;
                         esac
                       fi
                     changed_when: false
                 
                   - name: Get group with GID
                     command: "getent group {{ TCTOS_GID }}"
                     register: gid_lookup
                     changed_when: false
                     failed_when: false
                 
                   - name: Get group name from GID
                     set_fact:
                       gid_owner: "{{ gid_lookup.stdout.split(':')[0] }}"
                     when: gid_lookup.stdout != ""
                 
                   - name: Remove group
                     ansible.builtin.group:
                       name: "{{ gid_owner }}"
                       state: absent
                     when:
                       - gid_lookup.stdout != ""
                       - gid_owner != "users"
                 
                   - name: Create new group
                     ansible.builtin.group:
                       name: "users"
                       gid: "{{ TCTOS_GID }}"
                       state: present
                 
                   - name: Get user with UID
                     command: "getent passwd {{ TCTOS_UID }}"
                     register: uid_lookup
                     changed_when: false
                     failed_when: false
                 
                   - name: Get username from UID
                     set_fact:
                       uid_owner: "{{ uid_lookup.stdout.split(':')[0] }}"
                     when: uid_lookup.stdout != ""
                 
                   - name: Remove user
                     ansible.builtin.user:
                       name: "{{ uid_owner }}"
                       state: absent
                       remove: true
                     when:
                       - uid_lookup.stdout != ""
                       - uid_owner != "user"
                 
                   - name: Create new user
                     ansible.builtin.user:
                       name: "user"
                       uid: "{{ TCTOS_UID }}"
                       group: "users"
                       state: present
                       create_home: true
                       
                   - name: Install simlog
                     ansible.builtin.get_url:
                       url: "https://raw.githubusercontent.com/MohrJonas/Simlog/refs/heads/master/simlog"
                       dest: "/sbin/simlog"
                       mode: "0544"
                       owner: root
                       group: root
               """;
    }
}