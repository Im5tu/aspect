+++
title = "aspect resources"
description = ""
weight = 10
+++

`aspect resources [ARGUMENTS]` provides you an easy way of looking at the currently available resources for all the cloud providers registered in the system. If you are only interested in a specific provider, the follow the format `aspect resources <provider>` to limit the results to just that provider.

## Example output

{{< code lang="md" >}}
> aspect resources aws

┌────────────────────┬───────────┬──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┬──────────────────────────────────────────────────────────────────┐
│ Name               │ Provider  │ Description                                                                                                                                                                                                      │ Docs                                                             │
├────────────────────┼───────────┼──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┼──────────────────────────────────────────────────────────────────┤
│ AwsSecurityGroup   │ AWS       │ A security group acts as a virtual firewall for your instance to control inbound and outbound traffic. A security group specifies the actions that are allowed, not the actions that are blocked.                │ https://cloudaspect.app/docs/AWS/resources/AwsSecurityGroup/     │
└────────────────────┴───────────┴──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┴──────────────────────────────────────────────────────────────────┘

{{< /code >}}

## Arguments
{{< table style="table-striped" >}}
|Option|Position|Description|Default|Required?|
|---|---|---|---|---|
|provider|0|The specified provider to look at resources for. Options: `aws` / `azure`|No|
{{< /table >}}