+++
title = "Getting started"
description = ""
weight = 1
+++

The quickest way to get started is to use the prebuilt docker image:

```bash
docker run --rm -it im5tu/aspect:latest
```

From here you have full access to the `aspect` command line interface. Here are some commands that you may wish to execute:

- `aspect policy list builtin` - Lists all of the built in policies
- `aspect policy list view <policy name>.policy` - Views the contents of a policy
- `aspect policy init <policy name>.policy` - Creates an empty policy for a specified resource
- `aspect policy validate <policy name>.policy` - Ensures that the policy is valid
- `aspect run <policy name>.policy` - Runs the policy against your cloud infrastructure reporting its compliance

In order to construct a policy, checkout the [Policy Syntax documentation](/docs/getting-started/policy-syntax) to see how to construct a policy document and how to verify resources. 

For a full list of the commands and their available options, visit the [CLI Commands Documentation](/docs/getting-started/commands/). Please note, you may also need to configure cloud specific credentials. See the instructions for [AWS](/docs/aws/configuration/) and [Azure](/docs/azure/configuration/).