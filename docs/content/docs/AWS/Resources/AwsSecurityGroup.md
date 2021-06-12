+++
title = "AwsSecurityGroup"
description = "A security group acts as a virtual firewall for your instance to control inbound and outbound traffic. A security group specifies the actions that are allowed, not the actions that are blocked."
weight = 14
+++

A security group acts as a virtual firewall for your instance to control inbound and outbound traffic. A security group specifies the actions that are allowed, not the actions that are blocked.

## Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Account|The account that is associated with the resource.|[AwsAccount](/docs/aws/resources/awsaccount/)|
|Arn|The Amazon Resource Names (ARN) uniquely identifying an AWS resource.|String|
|Description|A friendly description of what the security group allows.|String|
|EgressRules|A series of rules that affect outbound traffic. There may be 0 or more entries in this collection.|Collection\<[Rule](#rule)>|
|Id|The ID of the security group.|String|
|IngressRules|A series of rules that affect inbound traffic. There may be 0 or more entries in this collection.|Collection\<[Rule](#rule)>|
|Name|The name of the resource.|String|
|OwnerId|The AWS account ID of the owner of the security group.|String|
|Region|The region in which this resource is located.|String|
|Tags|The tags that are associated with the resource. Each entry has two properties: Key/Value. There may be 0 or more entries in this collection.|Collection\<KeyValuePair<String, String>>|
|Type|The type of the resource. This is what is used in policy evaluation.|String|
|VpcId|The ID of the VPC for the security group.|String|
{{< /table >}}

## Policy Template
This template will give you a quick head start on generating a policy for an AwsSecurityGroup:

{{< code lang="tf" >}}
resource "AwsSecurityGroup"

validate {

}
{{< /code >}}
## Nested Types
### Rule
Describes a set of permissions for a security group.

#### Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|FromPort|The start of port range for the TCP and UDP protocols, or an ICMP/ICMPv6 type number. A value of -1 indicates all ICMP/ICMPv6 types.|Number|
|IPV4Ranges|A collection of IPV4 CIDR ranges that are allowed to communicate with this security group. There may be 0 or more entries in this collection.|Collection\<[IPV4Entry](#ipv4entry)>|
|IPV6Ranges|A collection of IPV6 CIDR ranges that are allowed to communicate with this security group. There may be 0 or more entries in this collection.|Collection\<[IPV6Entry](#ipv6entry)>|
|Prefixes|A prefix list is a set of one or more CIDR blocks. You can use prefix lists to make it easier to configure and maintain your security groups and route tables. You can create a prefix list from the IP addresses that you frequently use, and reference them as a set in security group rules and routes instead of referencing them individually. There may be 0 or more entries in this collection.|Collection\<[PrefixEntry](#prefixentry)>|
|Protocol|The protocol to allow. The most common protocols are 6 (TCP), 17 (UDP), and 1 (ICMP). -1 indicates all protocols.|String|
|SecurityGroups|A collection of other security groups that are allowed to communicate with this security group. There may be 0 or more entries in this collection.|Collection\<[SecurityGroupEntry](#securitygroupentry)>|
|ToPort|The end of port range for the TCP and UDP protocols, or an ICMP/ICMPv6 type number. A value of -1 indicates all ICMP/ICMPv6 types.|Number|
{{< /table >}}

### IPV4Entry
Describes a IPV4 CIDR range that is allowed to talk to the defined security group.

#### Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|CIDR|The IPV4 classless inter-domain routing (CIDR) range associated with this entry.|String|
|Description|A friendly description of what the CIDR range belongs to.|String|
{{< /table >}}

### IPV6Entry
Describes a IPV6 CIDR range that is allowed to talk to the defined security group.

#### Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|CIDR|The IPV6 classless inter-domain routing (CIDR) range associated with this entry.|String|
|Description|A friendly description of what the CIDR range belongs to.|String|
{{< /table >}}

### PrefixEntry
Describes a prefix list that is allowed to talk to the defined security group.

#### Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Description|A friendly description of what the prefix entry belongs to.|String|
|Id|The id of the prefix entry.|String|
{{< /table >}}

### SecurityGroupEntry
Describes a security group that is allowed to talk to the defined security group.

#### Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Account|The account that the security group is associated with.|String|
|Description|A friendly description of what the security group does.|String|
|Id|The id of the security group.|String|
|Name|The name of the security group.|String|
|VpcId|The VPC that the security group is associated with.|String|
|VpcPeeringConnectionId|The VPC peering connection the security group is associated with.|String|
{{< /table >}}

