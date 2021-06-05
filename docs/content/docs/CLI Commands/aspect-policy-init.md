+++
title = "aspect policy init"
description = "Create either a new policy file or policy suite"
weight = 6
+++

`aspect policy init [ARGUMENTS] [OPTIONS]` creates either a new policy file or policy suite with default contents ready for you to edit. If you do not specify the `-r|--resource` option, the UI will guide you through selecting a resource to get you started quicker.

## Arguments
{{< table style="table-striped" >}}
|Option|Position|Description|Default|Required?|
|---|---|---|---|---|
|filname|0|The filename where the template policy will be created||true|
{{< /table >}}

## Options

{{< table style="table-striped" >}}
|Option|Alias|Description|
|---|---|---|
|-d|--display|Displays the generated policy in the console.|
|-r|--resource|Skips the UI selection of the resource if the resource is known ahead of time. This is only valid for policy files, not policy suites.|
|-s|--suite|Create a policy suite.|
{{< /table >}}
