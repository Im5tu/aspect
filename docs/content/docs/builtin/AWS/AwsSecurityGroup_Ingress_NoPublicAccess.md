+++
title = "AwsSecurityGroup_Ingress_NoPublicAccess.policy"
weight = 19
+++

## Commands

{{< table style="table-striped" >}}
|Command|Description|
|------|------|
|`aspect policy view BuiltIn\AWS\AwsSecurityGroup_Ingress_NoPublicAccess.policy`|View the contents of the policy|
|`aspect policy validate BuiltIn\AWS\AwsSecurityGroup_Ingress_NoPublicAccess.policy`|Validate the policy|
|`aspect run BuiltIn\AWS\AwsSecurityGroup_Ingress_NoPublicAccess.policy`|Validate the policy|
{{< /table >}}

## Policy Definition
{{< code lang="tf" >}}
resource "AwsSecurityGroup"

exclude {
    hasKey(input.Tags, "AspectIgnore")
    hasKey(input.Tags, "PubliclyAccessible")
}

validate {
    input.IngressRules[_].IPV4Ranges[_].CIDR != "0.0.0.0/0"
}

{{< /code >}}

