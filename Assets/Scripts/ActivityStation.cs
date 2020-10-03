using System.Collections.Generic;

using UnityEngine;

namespace DefaultNamespace {
    public class ActivityStation : MonoBehaviour {
        public List<AstroAI> Assignees;

        public bool Occuppied => Assignees.Count != 0;
    }
}