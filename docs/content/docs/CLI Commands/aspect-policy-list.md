+++
title = "aspect policy list"
description = ""
weight = 7
+++

`aspect policy list [ARGUMENTS] [OPTIONS]` lists all of the policies within a specified source. If no source is specified, then the policies that come with your version of Aspect will be shown to you.

### Example output

{{< code lang="md" >}}
> aspect policy list builtin

┌────────────────────────────────────────────────┬──────────────────┬─────────┬──────────────┐
│ Policy                                         │ Resource         │ Created │ Last Updated │
├────────────────────────────────────────────────┼──────────────────┼─────────┼──────────────┤
│ BuiltIn\AWS\AWS_AllRegions_BestPractises.suite │ Various          │         │              │
│ BuiltIn\AWS\AWS_SecurityGroups_Tagging.policy  │ AwsSecurityGroup │         │              │
└────────────────────────────────────────────────┴──────────────────┴─────────┴──────────────┘
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
