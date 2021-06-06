+++
title = "aspect policy view"
description = ""
weight = 9
+++

`aspect policy view [ARGUMENTS]` enables you to view a the contents of a specific policy/policy suite, including the built in policies. The command will return the contents of the policy.


## Example output

{{< code lang="md" >}}
> aspect policy view D:\dev\personal\im5tu\Aspect\examples\test.policy

validate {
    input.Name == "default"
}

{{< /code >}}

## Arguments
{{< table style="table-striped" >}}
|Option|Position|Description|Default|Required?|
|---|---|---|---|---|
|source|0|Lists the policies and policy suites that can be found in the specified location. Source cannot be a directory.||No|
{{< /table >}}

## Exit Codes

{{< table style="table-striped" >}}
|Exit Code|Description|
|---|---|
|0|The policy is display successfully|
|1|The policy could not be found|
{{< /table >}}
