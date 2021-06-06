+++
title = "aspect watch"
description = ""
weight = 12
+++

`aspect watch [ARGUMENTS] [OPTIONS]` allows you to watch a specific directory or policy for changes and validate them as the files change. This decreases the total time required to validate policy changes during the development phase.


## Arguments

{{< table style="table-striped" >}}
|Option|Position|Description|Default|Required?|
|---|---|---|---|---|
|source|0|Lists the policies and policy suites that can be found in the specified location|current working directory|No|
{{< /table >}}

## Options

{{< table style="table-striped" >}}
|Option|Alias|Description|Default|
|---|---|---|---|
|--delay||The period of time in milliseconds that needs to elapse between changes to a file before the change is validated. This exists to work around underlying bugs in the .NET SDK. It's unlikely that you'll ever need to set this.|500|
{{< /table >}}
