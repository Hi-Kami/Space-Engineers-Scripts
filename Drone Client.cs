String OutputText = "";
int PairTicks = 0;
IMyTextPanel TextHandle;
IMyRadioAntenna Radio;
String ID = null;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    TextHandle = GridTerminalSystem.GetBlockWithName( "Right LCD" ) as IMyTextPanel;
    Radio = GridTerminalSystem.GetBlockWithName( "Drone Ant" ) as IMyRadioAntenna;
}

public void Save()
{
}

public void Main( string argument, UpdateType updateSource )
{
    if( ID == null )
    {
        if( PairTicks > 0 )
        {
            PairTicks--;
            TextHandle.WritePublicText( "Looking for Pair - " + PairTicks );
        } 

        if( argument == "PairMe" ){ PairTicks = 1000; }

        if( argument == "Pair" && PairTicks > 0 )
        {
            Radio.TransmitMessage( "PairMe", MyTransmitTarget.Everyone );
            TextHandle.WritePublicText( "Pairing"  );
            PairTicks = 0;
        }

        if( argument.Split( ':' )[0] == "Name" )
        {
            ID = argument.Split( ':' )[1];
            TextHandle.WritePublicText( "Pair Found" );
        }
    }
    //Output Max 18 lines 
}

