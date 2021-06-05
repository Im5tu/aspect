+++
title = "aspect policy validate"
description = ""
weight = 8
+++

`aspect policy validate [ARGUMENTS] [OPTIONS]` enables you to validate a specific file or directory to ensure that the policy is valid before running it using `aspect run`. If specifying a directory, the options listed below will also be available. The command will validate any policy file, policy suite or one of the built in policies.


### Example output

{{< code lang="md" >}}
> aspect policy validate D:\dev\personal\im5tu\Aspect\examples

┌───────────────────────────────────────────────────┬───────────┬────────┐
│ Source                                            │ Is Valid? │ Errors │
├───────────────────────────────────────────────────┼───────────┼────────┤
│ D:\dev\personal\im5tu\Aspect\examples\test.policy │ Valid     │        │
│ D:\dev\personal\im5tu\Aspect\examples\test.suite  │ Valid     │        │
└───────────────────────────────────────────────────┴───────────┴────────┘

{{< /code >}}

### Arguments
{{< table style="table-striped" >}}
|Option|Position|Description|Default|Required?|
|---|---|---|---|---|
|source|0|Lists the policies and policy suites that can be found in the specified location|builtin|No|
{{< /table >}}

### Options

{{< table style="table-striped" >}}
|Option|Alias|Description|
|---|---|---|
|-r|--recursive|Instructs the CLI to enumerate all child directories, not just the top level directory. Works for physical file systems only, ie: not `builtin`|
{{< /table >}}