+++
title = "AwsSecurityGroup_Egress_NoPublicAccess.policy"
weight = 18
+++

## Commands

{{< table style="table-striped" >}}
|Command|Description|
|------|------|
|`aspect policy view BuiltIn\AWS\AwsSecurityGroup_Egress_NoPublicAccess.policy`|View the contents of the policy|
|`aspect policy validate BuiltIn\AWS\AwsSecurityGroup_Egress_NoPublicAccess.policy`|Validate the policy|
|`aspect run BuiltIn\AWS\AwsSecurityGroup_Egress_NoPublicAccess.policy`|Validate the policy|
{{< /table >}}

## Policy Definition
{{< code lang="tf" >}}
resource "AwsSecurityGroup"

exclude {
    hasKey(input.Tags, "AspectIgnore")
    hasKey(input.Tags, "PubliclyAccessible")
}

validate {
    input.EgressRules[_].IPV4Ranges[_].CIDR != "0.0.0.0/0"
}

{{< /code >}}

