# Aspect

<!-- Badges -->
![GitHub license](https://img.shields.io/github/license/im5tu/aspect.svg?style=flat-square)
![GitHub release](https://img.shields.io/github/release/im5tu/aspect.svg?style=flat-square)
![GitHub last commit](https://img.shields.io/github/last-commit/im5tu/aspect?style=flat-square)
![GitHub contributors](https://img.shields.io/github/contributors/im5tu/aspect?style=flat-square)

Aspect is a simple rule based engine to ensure that cloud resources meet organisational requirements across multiple clouds and regions at the same time. The same rules based engine is available as a REPL to support arbitary sub-second evaulation of cloud resources.

## Features

- Comprehensive cloud provider support:
    * AWS
    * Azure (Coming Soon!)
- Cloud native authentication mechanisms
- Policy declaration language similar to OpenPolicy Agent's rego [See Examples](https://github.com/Im5tu/aspect/tree/main/examples)
- Policy suites written in YAML that supports both multiple clouds and regions [See Examples](https://github.com/Im5tu/aspect/tree/main/examples)
- Interactive policy builder
- Validate policies and policy suites at development time
- Fully interactive CLI for viewing and evaluating cloud resources

## Quickstart

The quickest way to get started is to use the prebuilt docker image:

```bash
docker run --rm -it im5tu/aspect:latest
```

From here you have full access to the `aspect` command line interface. Here are some commands that you may wish to execute:

- `aspect policy init myfirst.policy` - Creates an empty policy for a specified resource
- `aspect policy list builtin` - Lists all of the built in policies
- `aspect policy validate myfirst.policy` - Ensures that the policy is valid
- `aspect run myfirst.policy` - Runs the policy against your cloud infrastructure reporting its compliance

For a full list of the commands and their available options, visit the [CLI Commands Documentation](https://cloudaspect.app/docs/getting-started/commands/). Please note, you may also need to configure cloud specific credentials. See the instructions for [AWS](https://cloudaspect.app/docs/aws/configuration/) and [Azure](https://cloudaspect.app/docs/azure/configuration/).

## Supported Resources

- AWS
    - Security Groups

## Roadmap

See the [open issues](https://github.com/im5tu/aspect/issues) for a list of proposed features (and known issues).

## Built With

Here are some of the awesome community projects that make this project possible:

- [Spectre.Console](https://github.com/spectreconsole/spectre.console)
- [YamlDotNet](https://github.com/aaubry/YamlDotNet)
- [JSON.NET](https://www.newtonsoft.com/json)
- [MinVer](https://github.com/adamralph/minver)


## Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**.

Take a look at our [contribution guide](https://cloudaspect.app/docs/contributing/) for more details.