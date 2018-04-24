using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Othello.Main.Model
{
    public class PlaySetModel
    {
        List<CellModel> _cells;
        List<DiscModel> _discs;

        public PlaySetModel()
        {
            _cells = new List<CellModel>();
            _discs = new List<DiscModel>();
        }

        public IEnumerable<CellModel> Cells => _cells;
        public IEnumerable<DiscModel> Discs => _discs;

        public void Reset()
        {
            _cells.Clear();
            _discs.Clear();
            //IsUndo = false;
        }

        public void AddCell(CellModel cell)
        {
            var existing = _cells.Where(c => c==cell).ToList();
            foreach (var c in existing)
            {
                _cells.Remove(c);
            }
            _cells.Add(cell);
        }

        public void AddDisc(DiscModel disc)
        {
            var existing = _discs.Where(d => d==disc).ToList();
            foreach (var d in existing)
            {
                _discs.Remove(d);
            }
            _discs.Add(disc);
        }


    }
}
