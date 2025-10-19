using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

namespace ITCafe.CafeBusiness
{
    public class TableService
    {
        public bool HasFreeTable => _availableTables.Any();

        private readonly HashSet<Transform> _availableTables;

        public TableService([Key(Constants.CLIENT_SEATS)] IEnumerable<Transform> tableSpots)
        {
            _availableTables = new HashSet<Transform>(tableSpots);
            Debug.Log($"TableService has {_availableTables.Count} available tables");
        }

        public Transform GetFreeTable()
        {
            if (!_availableTables.Any())
                return null;

            var randomIndex = Random.Range(0, _availableTables.Count);
            var table = _availableTables.ElementAt(randomIndex);
    
            _availableTables.Remove(table);
    
            return table;
        }

        public void FreeTable(Transform table)
        {
            _availableTables.Add(table);
        }
    }
}