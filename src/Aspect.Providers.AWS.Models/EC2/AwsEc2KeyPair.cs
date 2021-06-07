using System.Collections.Generic;
using System.ComponentModel;

namespace Aspect.Providers.AWS.Models.EC2
{
    [Description("A key pair, consisting of a private key and a public key, is a set of security credentials that you use to prove your identity when connecting to an EC2 instance.")]
    public class AwsEc2KeyPair : AwsResource
    {
        [Description("The ID of the key pair.")]
        public string? Id { get; init; }

        [Description("If you used CreateKeyPair to create the key pair, this is the SHA-1 digest of the DER encoded private key. If you used ImportKeyPair to provide AWS the public key, this is the MD5 public key fingerprint as specified in section 4 of RFC4716.")]
        public string? Fingerprint { get; set; }

        public AwsEc2KeyPair(AwsAccount account, string arn, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string region)
            : base(account, arn, name, tags, nameof(AwsEc2KeyPair), region)
        {
        }
    }
}
