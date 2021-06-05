+++
title = "aspect policy build"
description = "Use the UI to create a policy file"
weight = 4
+++

{{< alert style="warning" >}} This is preview documentation for a future command. This command is not available in the released images. {{< /alert >}}

`aspect policy build [OPTIONS]` hosts a friendly interface for ensuring that you always build valid policy files. The UI will take you through the following flow:

- Selection of the cloud provider the policy is for
- Selection of the resource the policy is for
- Whether or not you would like to configure the additional `include` / `exclude` statement boxes
- Foreach of the statement blocks (`include` / `exclude` / `validate`), which statements you wish to configure

As you enter each statement, it is validated immediately giving you the chance to correct the policy fragment if required. If validation is succesfully, the fragment is added to the statement block.

Once you have completed all of the policy fragments, the policy is displayed on screen for you, and, if selected, written to the file destination of your choice.

## Options

{{< table style="table-striped" >}}
|Option|Alias|Description|
|---|---|---|
|-f|--filename|The filename where the policy will be written on completion. For a policy file, it must end with `.policy` and for a policy suite, it's `.suite`|
|--no-ui||Do not display the generated policy at the end of the process|
|-s|--suite|Create a new policy suite|
{{< /table >}}
