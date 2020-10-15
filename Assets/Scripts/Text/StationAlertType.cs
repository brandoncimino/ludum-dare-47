﻿namespace DefaultNamespace.Text {
    public enum StationAlertType {
        #region Behaving

        Behave_Kitchen,
        Behave_Bridge,
        Behave_Recreation,
        Behave_Lab,
        Behave_Engine,

        #endregion

        #region Misbehaving

        Misbehave_Kitchen,
        Misbehave_Bridge,
        Misbehave_Recreation,
        Misbehave_Lab,
        Misbehave_Engine,

        #endregion

        #region Astronaut Events

        Astronaut_Bored,
        Astronaut_Cloned,
        Astronaut_Dead_Generic,
        Astronaut_Dead_Fire,
        Astronaut_Dead_Monster,

        #endregion

        #region Station Events

        Station_TooFast,
        Station_TooSlow,

        #endregion
    }
}