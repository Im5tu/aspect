using System.Collections.Generic;
using System.ComponentModel;

namespace Aspect.Providers.AWS.Models.EC2
{
    [Description("The AMIs, AKIs, and ARIs available to you or all of the images available to you. The images available to you include public images, private images that you own, and private images owned by other AWS accounts for which you have explicit launch permissions.")]
    public class AwsEc2Image : AwsResource
    {
        [Description("The architecture of the image.")]
        public string? Architecture { get; init; }

        [Description("The boot mode of the image. For more information, see \"https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/ami-boot.html\" (Boot modes) in the Amazon Elastic Compute Cloud User Guide.")]
        public string? BootMode { get; init; }

        [Description("The description of the AMI that was provided during image creation.")]
        public string? Description { get; init; }

        [Description("Specifies whether enhanced networking with ENA is enabled.")]
        public bool EnaSupport { get; init; }

        [Description("The hypervisor type of the image.")]
        public string? Hypervisor { get; init; }

        [Description("The ID of the AMI.")]
        public string? ImageId { get; init; }

        [Description("The location of the AMI.")]
        public string? ImageLocation { get; init; }

        [Description("The type of image.")]
        public string? ImageType { get; init; }

        [Description("Indicates whether the image has public launch permissions.")]
        public bool IsPublic { get; init; }

        [Description("The kernel associated with the image, if any. Only applicable for machine images.")]
        public string? KernelId { get; init; }

        [Description("The AWS account ID of the image owner.")]
        public string? OwnerId { get; init; }

        [Description("This value is set to windows for Windows AMIs; otherwise, it is blank.")]
        public string? Platform { get; init; }

        [Description("The platform details associated with the billing code of the AMI.")]
        public string? PlatformDetails { get; init; }

        [Description("The RAM disk associated with the image, if any. Only applicable for machine images.")]
        public string? RamdiskId { get; init; }

        [Description("The device name of the root device volume (eg: '/dev/sda1')")]
        public string? RootDeviceName { get; init; }

        [Description("The type of root device used by the AMI. The AMI can use an EBS volume or an instance store volume.")]
        public string? RootDeviceType { get; init; }

        [Description("Specifies whether enhanced networking with the Intel 82599 Virtual Function interface is enabled.")]
        public string? SriovNetSupport { get; init; }

        [Description("The current state of the AMI. If the state is available, the image is successfully registered and can be used to launch an instance.")]
        public string? State { get; init; }

        [Description("The type of virtualization of the AMI.")]
        public string? VirtualizationType { get; init; }

        public AwsEc2Image(AwsAccount account, string arn, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string region)
            : base(account, arn, name, tags, nameof(AwsEc2Image), region)
        {
        }
    }
}
