﻿using System;
using System.Collections.Generic;

namespace Aspect.Abstractions
{
    /// <summary>
    ///     Represents a resource in a cloud provider
    /// </summary>
    public interface IResource : IEquatable<IResource>
    {
        /// <summary>
        ///     The name of the resource
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The cloud specific unique identifier for this resource, eg: the AWS ARN
        /// </summary>
        string CloudId { get; }

        /// <summary>
        ///     The type of the resource to be referenced in policies
        /// </summary>
        string Type { get; }

        /// <summary>
        ///     The region that the resource is located in
        /// </summary>
        string Region { get; }

        /// <summary>
        ///     The tags associated with the resource
        /// </summary>
        IReadOnlyList<KeyValuePair<string, string>> Tags { get; }
    }

    /// <summary>
    ///     Represents a resource in a cloud provider
    /// </summary>
    public interface IResource<TAccount, TAccountIdentifier> : IResource
        where TAccount : Account<TAccountIdentifier>
        where TAccountIdentifier : AccountIdentifier
    {
        /// <summary>
        ///     The account that the resource is located in
        /// </summary>
        TAccount Account { get; }
    }
}
