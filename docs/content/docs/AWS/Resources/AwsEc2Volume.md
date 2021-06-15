+++
title = "AwsEc2Volume"
description = "An Amazon EBS volume is a durable, block-level storage device that you can attach to your instances. After you attach a volume to an instance, you can use it as you would use a physical hard drive. "
weight = 6
+++

An Amazon EBS volume is a durable, block-level storage device that you can attach to your instances. After you attach a volume to an instance, you can use it as you would use a physical hard drive. 

## Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Account|The account that is associated with the resource.|[AwsAccount](/docs/aws/resources/awsaccount/)|
|Attachments|Describes the attachments to an EC2 instances. There may be 0 or more entries in this collection.|Collection\<[VolumeAttachment](#volumeattachment)>|
|AvailabilityZone|The Availability Zone for the volume.|String|
|CloudId|The cloud specific unique identifier for this resource, eg: AWS Arn.|String|
|Encrypted|Indicates whether the volume is encrypted.|Boolean|
|FastRestored|Indicates whether the volume was created using fast snapshot restore.|Boolean|
|Id|The ID of the volume.|String|
|Iops|The number of I/O operations per second (IOPS). For gp3, io1 and io2 volumes, this represents the number of IOPS that are provisioned for the volume. For gp2 volumes, this represents the baseline performance of the volume and the rate at which the volume accumulates I/O credits for bursting.|Number|
|KmsKeyId|The Amazon Resource Name (ARN) of the AWS Key Management Service (AWS KMS) customer master key (CMK) that was used to protect the volume encryption key for the volume.|String|
|MultiAttachEnabled|Indicates whether Amazon EBS Multi-Attach is enabled.|Boolean|
|Name|The name of the resource.|String|
|OutpostArn|The Amazon Resource Name (ARN) of the Outpost.|String|
|Region|The region in which this resource is located.|String|
|Size|The size of the volume, in GiBs.|Number|
|SnapshotId|The snapshot from which the volume was created, if applicable.|String|
|State|The volume state.|String|
|Tags|The tags that are associated with the resource. Each entry has two properties: Key/Value. There may be 0 or more entries in this collection.|Collection\<KeyValuePair<String, String>>|
|Throughput|The throughput that the volume supports, in MiB/s.|Number|
|Type|The type of the resource. This is what is used in policy evaluation.|String|
|VolumeType|The volume type.|String|
{{< /table >}}

## Policy Template
This template will give you a quick head start on generating a policy for an AwsEc2Volume:

{{< code lang="tf" >}}
resource "AwsEc2Volume"

validate {

}
{{< /code >}}
## Nested Types
### VolumeAttachment
Describes the attachment to an EC2 instance.

#### Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|DeleteOnTermination|Indicates whether the EBS volume is deleted on instance termination.|Boolean|
|Device|The device name.|String|
|InstanceId|The ID of the instance.|String|
|State|The attachment state of the volume.|String|
|VolumeId|The ID of the volume.|String|
{{< /table >}}

