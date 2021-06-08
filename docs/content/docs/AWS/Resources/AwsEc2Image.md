+++
title = "AwsEc2Image"
description = "The AMIs, AKIs, and ARIs available to you or all of the images available to you. The images available to you include public images, private images that you own, and private images owned by other AWS accounts for which you have explicit launch permissions."
weight = 2
+++

The AMIs, AKIs, and ARIs available to you or all of the images available to you. The images available to you include public images, private images that you own, and private images owned by other AWS accounts for which you have explicit launch permissions.

## Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Account|The account that is associated with the resource.|[AwsAccount](/docs/aws/resources/awsaccount/)|
|Architecture|The architecture of the image.|String|
|Arn|The Amazon Resource Names (ARN) uniquely identifying an AWS resource.|String|
|BootMode|The boot mode of the image. For more information, see "https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/ami-boot.html" (Boot modes) in the Amazon Elastic Compute Cloud User Guide.|String|
|Description|The description of the AMI that was provided during image creation.|String|
|EnaSupport|Specifies whether enhanced networking with ENA is enabled.|Boolean|
|Hypervisor|The hypervisor type of the image.|String|
|ImageId|The ID of the AMI.|String|
|ImageLocation|The location of the AMI.|String|
|ImageType|The type of image.|String|
|IsPublic|Indicates whether the image has public launch permissions.|Boolean|
|KernelId|The kernel associated with the image, if any. Only applicable for machine images.|String|
|Name|The name of the resource.|String|
|OwnerId|The AWS account ID of the image owner.|String|
|Platform|This value is set to windows for Windows AMIs; otherwise, it is blank.|String|
|PlatformDetails|The platform details associated with the billing code of the AMI.|String|
|RamdiskId|The RAM disk associated with the image, if any. Only applicable for machine images.|String|
|Region|The region in which this resource is located.|String|
|RootDeviceName|The device name of the root device volume (eg: '/dev/sda1').|String|
|RootDeviceType|The type of root device used by the AMI. The AMI can use an EBS volume or an instance store volume.|String|
|SriovNetSupport|Specifies whether enhanced networking with the Intel 82599 Virtual Function interface is enabled.|String|
|State|The current state of the AMI. If the state is available, the image is successfully registered and can be used to launch an instance.|String|
|Tags|The tags that are associated with the resource. Each entry has two properties: Key/Value. There may be 0 or more entries in this collection.|Collection\<KeyValuePair<String, String>>|
|Type|The type of the resource. This is what is used in policy evaluation.|String|
|VirtualizationType|The type of virtualization of the AMI.|String|
{{< /table >}}

## Policy Template
This template will give you a quick head start on generating a policy for an AwsEc2Image:

{{< code lang="tf" >}}
resource "AwsEc2Image"

validate {

}
{{< /code >}}
