+++
title = "AWS_AllRegions_BestPractises.suite"
weight = 1
+++

## Commands

{{< table style="table-striped" >}}
|Command|Description|
|------|------|
|`aspect policy view BuiltIn\AWS\AWS_AllRegions_BestPractises.suite`|View the contents of the policy|
|`aspect policy validate BuiltIn\AWS\AWS_AllRegions_BestPractises.suite`|Validate the policy|
|`aspect run BuiltIn\AWS\AWS_AllRegions_BestPractises.suite`|Validate the policy|
{{< /table >}}

## Policy Definition
{{< code lang="yml" >}}
name: AWS Best Practises
description: Includes some best practises for AWS resources, such as tagging, egress rules and more.
policies:
    -   type: AWS
        name: All built in policies
        policies:
            - BuiltIn\AWS\

{{< /code >}}

