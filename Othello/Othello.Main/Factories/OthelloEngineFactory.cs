﻿using System;
using Othello.Main.Engine;
using System.Collections.Generic;
using Othello.Main.Model;

namespace Othello.Main.Factories
{
    public class OthelloEngineFactory
    {
        Func<OthelloEngine> _creator;

        public OthelloEngineFactory(Func<OthelloEngine> creator)
        {
            _creator = creator;
        }

        public OthelloEngine Create(IEnumerable<CellModel> cells)
        {
            var vm = _creator();
            vm.Initialize(cells);
            return vm;
        }

    }
}
