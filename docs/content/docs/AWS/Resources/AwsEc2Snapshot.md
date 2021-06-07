+++
title = "AwsEc2Snapshot"
description = "Amazon EBS provides the ability to create snapshots (backups) of any EBS volume. A snapshot takes a copy of the EBS volume and places it in Amazon S3, where it is stored redundantly in multiple Availability Zones."
weight = 5
+++

Amazon EBS provides the ability to create snapshots (backups) of any EBS volume. A snapshot takes a copy of the EBS volume and places it in Amazon S3, where it is stored redundantly in multiple Availability Zones.

## Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Account|The account that is associated with the resource.|[AwsAccount](/docs/aws/resources/awsaccount/)|
|Arn|The Amazon Resource Names (ARN) uniquely identifying an AWS resource.|String|
|DataEncryptionKeyId|The data encryption key identifier for the snapshot. This value is a unique identifier that corresponds to the data encryption key that was used to encrypt the original volume or snapshot copy. Because data encryption keys are inherited by volumes created from snapshots, and vice versa, if snapshots share the same data encryption key identifier, then they belong to the same volume/snapshot lineage.|String|
|Description|The description for the snapshot.|String|
|Encrypted|Indicates whether the snapshot is encrypted.|Boolean|
|Id|The ID of the snapshot.|String|
|KmsKeyId|The Amazon Resource Name (ARN) of the AWS Key Management Service (AWS KMS) customer master key (CMK) that was used to protect the volume encryption key for the parent volume.|String|
|Name|The name of the resource.|String|
|OutpostArn|The ARN of the AWS Outpost on which the snapshot is stored.|String|
|OwnerId|The AWS account ID of the EBS snapshot owner.|String|
|Progress|The progress of the snapshot, as a percentage.|String|
|Region|The region in which this resource is located.|String|
|State|The snapshot state.|String|
|Tags|The tags that are associated with the resource. Each entry has two properties: Key/Value. There may be 0 or more entries in this collection.|Collection\<KeyValuePair<String, String>>|
|Type|The type of the resource. This is what is used in policy evaluation.|String|
|VolumeId|The ID of the volume that was used to create the snapshot.|String|
|VolumeSize|The size of the volume, in GiB.|Number|
{{< /table >}}

## Policy Template
This template will give you a quick head start on generating a policy for an AwsEc2Snapshot:

{{< code lang="tf" >}}
resource "AwsEc2Snapshot"

validate {

}
{{< /code >}}
