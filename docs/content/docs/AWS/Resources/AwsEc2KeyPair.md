+++
title = "AwsEc2KeyPair"
description = "A key pair, consisting of a private key and a public key, is a set of security credentials that you use to prove your identity when connecting to an EC2 instance."
weight = 4
+++

A key pair, consisting of a private key and a public key, is a set of security credentials that you use to prove your identity when connecting to an EC2 instance.

## Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Account|The account that is associated with the resource.|[AwsAccount](/docs/aws/resources/awsaccount/)|
|Arn|The Amazon Resource Names (ARN) uniquely identifying an AWS resource.|String|
|Fingerprint|If you used CreateKeyPair to create the key pair, this is the SHA-1 digest of the DER encoded private key. If you used ImportKeyPair to provide AWS the public key, this is the MD5 public key fingerprint as specified in section 4 of RFC4716.|String|
|Id|The ID of the key pair.|String|
|Name|The name of the resource.|String|
|Region|The region in which this resource is located.|String|
|Tags|The tags that are associated with the resource. Each entry has two properties: Key/Value. There may be 0 or more entries in this collection.|Collection\<KeyValuePair<String, String>>|
|Type|The type of the resource. This is what is used in policy evaluation.|String|
{{< /table >}}

## Policy Template
This template will give you a quick head start on generating a policy for an AwsEc2KeyPair:

{{< code lang="tf" >}}
resource "AwsEc2KeyPair"

validate {

}
{{< /code >}}
