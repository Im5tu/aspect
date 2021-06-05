+++
title = "aspect run"
description = ""
weight = 10
+++

`aspect run [ARGUMENTS] [OPTIONS]` allows you to run any policy or policy suite against your cloud infrastructure. If the policies being processed are valid, the CLI will generate a a JSON file with the failed resources for you to process.

If you are running a policy file, the CLI will respect the provider specific environment variables such as AWS's `AWS_DEFAULT_REGION` environment variable.

## Example output

{{< code lang="json" >}}
{
  "Errors": [],
  "FailedResources": [
    {
      "Resource": {
        "Name": "launch-wizard-3",
        "Type": "AwsSecurityGroup",
        "IngressRules": [],
        "EgressRules": [],
        "Arn": "arn:aws:ec2:ca-central-1:132456798012:security-group/sg-132456798012",
        "Account": {
          "Id": {
            "Id": "132456798012",
            "Name": "my-aws-account"
          },
          "Type": "AWS"
        },
        "Region": "ca-central-1",
        "Tags": []
      },
      "Source": "D:\\dev\\personal\\im5tu\\Aspect\\examples\\test.policy"
    }
  ]
}
{{< /code >}}

### Invalid policy output

{{< code lang="md" >}}
┌───────────────────────────────────────────────────┬─────────┬──────────────────────────────────────────────────┐
│ Policy                                            │ IsValid │ Errors                                           │
├───────────────────────────────────────────────────┼─────────┼──────────────────────────────────────────────────┤
│ D:\dev\personal\im5tu\Aspect\examples\test.suite  │ Invalid │ - A policy suite must have a top level name set. │
└───────────────────────────────────────────────────┴─────────┴──────────────────────────────────────────────────┘
{{< /code >}}

## Arguments

{{< table style="table-striped" >}}
|Option|Position|Description|Default|Required?|
|---|---|---|---|---|
|source|0|Lists the policies and policy suites that can be found in the specified location|current working directory|No|
{{< /table >}}

## Exit Codes

{{< table style="table-striped" >}}
|Exit Code|Description|
|---|---|
|-1|One or more resources failed policy evaluation|
|0|Ran to completion with no failed resources or errors|
|1|Policy could not be validated successfully|
|2|The policy run returned errors|
{{< /table >}}
