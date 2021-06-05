---
title: "Configuration"
date: 2021-05-30T22:10:26+01:00
weight: 1
---

Aspect follows the same credential discovery flow as the AWS CLI since Aspect is based ontop of the AWS SDK for .NET. The search for credentials is in the following order and uses the first available set for the current application:

1. A credentials profile with the name specified by a value in environment variable `AWS_PROFILE`.
1. The default credentials profile.
1. SessionAWSCredentials that are created from the `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`, and `AWS_SESSION_TOKEN` environment variables, if they’re all non-empty.
1. BasicAWSCredentials that are created from the `AWS_ACCESS_KEY_ID` and `AWS_SECRET_ACCESS_KEY` environment variables, if they’re both non-empty.
1. IAM Roles for Tasks for Amazon ECS tasks.
1. EC2 instance metadata.

This means that you can follow your standard AWS practises, such as using [gimme-aws-creds](https://github.com/Nike-Inc/gimme-aws-creds) and assuming different roles.

## Docker

### Credentials File

If you are using a credentials file, you can mount it in as a readonly directory into docker and Aspect will use this credentials file for its execution:

```bash
docker run --rm -it -v $HOME/.aws:/root/.aws:ro im5tu/aspect:latest
```

If you wanted to specify a different profile, specify the environment variable `AWS_PROFILE` as you start the container:

```bash
docker run --rm -it -e AWS_PROFILE=MyOtherProfile -v $HOME/.aws:/root/.aws:ro im5tu/aspect:latest
```

### Using Environment Variables

{{< alert style="warning" >}} This approach is not recommened as the credentials could be pulled from the environment variables. {{< / alert >}}

You may also specify the environment variables to configure the AWS credentials, for example, setting it on the container start would be:

```bash
docker run --rm -it -e AWS_PROFILE=MyOtherProfile -e AWS_ACCESS_KEY_ID=XXX -e AWS_SECRET_ACCESS_KEY=XXX im5tu/aspect:latest
```