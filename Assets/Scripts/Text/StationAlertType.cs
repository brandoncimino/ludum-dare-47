﻿namespace DefaultNamespace.Text {
    public enum StationAlertType {
        #region captions for the console

        Console_Line_Generic,
        Console_Line_Station,
        Console_Line_Astronaut,
        Data_1,
        Data_2,

        #endregion

        #region Station Updates

        Room_Behave_Kitchen,
        Room_Behave_Bridge,
        Room_Behave_Recreation,
        Room_Behave_Lab,
        Room_Behave_Engine,

        Room_Misbehave_Kitchen,
        Room_Misbehave_Bridge,
        Room_Misbehave_Recreation,
        Room_Misbehave_Lab,
        Room_Misbehave_Engine,

        #endregion

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
        Astronaut_Fleeing,
        Astronaut_Idle,
        Astronaut_Moving_Behaving,
        Astronaut_Moving_Misbehaving,

        #endregion

        #region Station Events

        Station_TooFast,
        Station_TooSlow,

        #endregion

        #region Monster

        Monster_Spawn

        #endregion
    }
}