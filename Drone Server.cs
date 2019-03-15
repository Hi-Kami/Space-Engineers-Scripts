List <IMyTextPanel> TextHandle;
List <IMyShipConnector> DockingHandle;
IMyRadioAntenna Radio;
int CurrentDrones = 0;
Vector3D CurrentPosition;
float Ticks = 0.0f;
float PairCount = 0;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    TextHandle = new List<IMyTextPanel>();
    TextHandle.Add( GridTerminalSystem.GetBlockWithName( "Drone LCD Small" ) as IMyTextPanel );
    TextHandle.Add( GridTerminalSystem.GetBlockWithName( "Drone LCD Norm" ) as IMyTextPanel );
    TextHandle.Add( GridTerminalSystem.GetBlockWithName( "Drone LCD Wide" ) as IMyTextPanel );

    DockingHandle = new List<IMyShipConnector>();
    DockingHandle.Add( GridTerminalSystem.GetBlockWithName( "Drone Dock 1" ) as IMyShipConnector );
    DockingHandle.Add( GridTerminalSystem.GetBlockWithName( "Drone Dock 2" ) as IMyShipConnector );

    Radio = GridTerminalSystem.GetBlockWithName( "Drone Ant" ) as IMyRadioAntenna;
}

public void Save()
{
    Ticks = 0.0f;
}

public void Main( string argument, UpdateType updateSource )
{
    /* if( argument == "Detected" )
    {
        Thrusters[0].SetValue<float>( "Override", 3000.0f );
        Thrusters[1].SetValue<float>( "Override", 3000.0f );
        DrillHandle1.GetActionWithName( "OnOff_On" ).Apply( DrillHandle1 ); 
        DrillHandle2.GetActionWithName( "OnOff_On" ).Apply( DrillHandle2 ); 
        Rotor.GetActionWithName( "OnOff_On" ).Apply( Rotor ); 
        TextHandle[1].WritePublicText( "Detected" );
    }
    else if( argument == "None" )
    {
        Thrusters[0].SetValue<float>( "Override", 0.0f );
        Thrusters[1].SetValue<float>( "Override", 0.0f );
        DrillHandle1.GetActionWithName( "OnOff_Off" ).Apply( DrillHandle1 );
        DrillHandle2.GetActionWithName( "OnOff_Off" ).Apply( DrillHandle2 );
        Rotor.GetActionWithName( "OnOff_Off" ).Apply( Rotor ); 
        TextHandle[1].WritePublicText( "Not Detected" ); 
    } */

    if( argument == "Pair" )
    {
        PairCount = 1000;
    }

    if( argument == "PairMe" & PairCount > 1 )
    {
        Radio.TransmitMessage( "Name:" + CurrentDrones, MyTransmitTarget.Everyone );
        TextHandle[1].WritePublicText( "Assigned Pair" );
        CurrentDrones++;
        PairCount = 0;
    }

    if( PairCount > 1 )
    {
        Radio.TransmitMessage( "Pair", MyTransmitTarget.Everyone );
        PairCount--;
        TextHandle[1].WritePublicText( "Looking for Pair - " + PairCount );
    }
    

    //CurrentPosition = Me.GetPosition();
    //TextHandle[0].WritePublicText( "X: " + CurrentPosition.X.ToString() + "\n" + "Y: " + CurrentPosition.Y.ToString() + "\n" +  "Z: " + CurrentPosition.Z.ToString() ); 
    Ticks++;
    TextHandle[0].WritePublicText( "System Online - Current Drones: " + CurrentDrones + "\nLast Saved " + Ticks + " Ticks ago" );
}
