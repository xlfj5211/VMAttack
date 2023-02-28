﻿using System.Collections.Generic;
using VMAttack.Core;
using VMAttack.Core.Abstraction;
using VMAttack.Pipeline.VirtualMachines.EzirizVM.Architecture;

namespace VMAttack.Pipeline.VirtualMachines.EzirizVM.Disassembly;

public class Disassembler : ContextBase
{
    private readonly MethodDecoder _methodDecoder;
    private readonly Dictionary<uint, EzirizMethod> _methods = new();

    public Disassembler(Context context, EzirizStreamReader ezirizStreamReader)
        : base(context, context.Logger)
    {
        if (!ezirizStreamReader.ManifestResource.TryGetReader(out var reader))
            throw new DevirtualizationException("Cannot create reader for disassembler!");

        EzirizStreamReader = ezirizStreamReader;
        _methodDecoder = new MethodDecoder(context, reader, ezirizStreamReader);
    }

    public EzirizStreamReader EzirizStreamReader { get; }

    public EzirizMethod GetOrCreateMethod(uint id, ulong methodOffset)
    {
        if (!_methods.TryGetValue(id, out var method))
        {
            Logger.Debug($"Created new method_{id:X4} with entry key {methodOffset:X8}.");

            var disassembled = _methodDecoder.CreateMethod(id, methodOffset);

            _methods.Add(id, disassembled);
        }

        return method!;
    }
}