﻿using System;
using System.Collections.Generic;

namespace Plato.Internal.Data.Schemas.Abstractions.Builders
{
    public interface ISchemaBuilderBase
    {

        ICollection<string> Statements { get; }

        SchemaBuilderOptions Options { get; }

        ISchemaBuilderBase Configure(Action<SchemaBuilderOptions> configure);

    }

}
