List <IMyPistonBase> Pistons;
IMyShipMergeBlock MergeBlock;
IMyLandingGear Holder;
IMyTextPanel TextHandle;
IMyProjector ProjectorHandle;
int Ticks = 250;

int State = 0;
bool Stop = false;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
    
    Pistons = new List<IMyPistonBase>();
    Pistons.Add( GridTerminalSystem.GetBlockWithName( "Piston X" ) as IMyPistonBase );
    Pistons.Add( GridTerminalSystem.GetBlockWithName( "Piston Y" ) as IMyPistonBase );

    MergeBlock = GridTerminalSystem.GetBlockWithName( "Merge Block Important" ) as IMyShipMergeBlock;

    Holder = GridTerminalSystem.GetBlockWithName( "Landing Gear Piston Lock" ) as IMyLandingGear; 

    TextHandle = GridTerminalSystem.GetBlockWithName( "LCD" ) as IMyTextPanel; 
    
    ProjectorHandle = GridTerminalSystem.GetBlockWithName( "Projector" ) as IMyProjector; 
}

public void Save()
{
}



public void Main( string argument, UpdateType updateSource )
{
    if( argument == "Start" )
    { 
        State = 1; 
        Pistons[0].GetActionWithName( "Extend" ).Apply( Pistons[0] );
    }
    
    if( argument == "Stop" )
    {
        Stop = true;
    }

    if( State == 0 ){ TextHandle.WritePublicText( "Offline" ); }

    //Extend Piston X
    if( State == 1 )
    {
        if( float.Parse( Pistons[0].DetailedInfo.Split( ':' )[1].Split( 'm' )[0] ) > Pistons[0].GetValue<float>( "UpperLimit" ) - 0.01f )
        {
            State = 2;
            Holder.GetActionWithName( "Lock" ).Apply( Holder );
            Pistons[0].GetActionWithName( "Retract" ).Apply( Pistons[0] );
            MergeBlock.GetActionWithName( "OnOff_Off" ).Apply( MergeBlock );
        }
        TextHandle.WritePublicText( "Extend Piston X 1" );
    }
    //Retract Piston X
    if( State == 2 )
    {
        if( float.Parse( Pistons[0].DetailedInfo.Split( ':' )[1].Split( 'm' )[0] ) < Pistons[0].GetValue<float>( "LowerLimit" ) + 0.01f )
        {
            State = 3;
            Pistons[1].GetActionWithName( "Extend" ).Apply( Pistons[1] );
            MergeBlock.GetActionWithName( "OnOff_On" ).Apply( MergeBlock );
            ProjectorHandle.GetActionWithName( "OnOff_On" ).Apply( ProjectorHandle );//Here
        }
        TextHandle.WritePublicText( "Retract Piston X 2" );
    }
    //Y Extend
    if( State == 3 )
    {
        if( float.Parse( Pistons[1].DetailedInfo.Split( ':' )[1].Split( 'm' )[0] ) > Pistons[1].GetValue<float>( "UpperLimit" ) - 0.01f )
        {
            State = 4;
            Ticks = 250;
            Pistons[1].GetActionWithName( "Retract" ).Apply( Pistons[1] );
        }
        TextHandle.WritePublicText( "Extend Piston Y 3" );
    }
    if( State == 4 )
    {
        if( Ticks < 1 )
        {
            Holder.GetActionWithName( "Unlock" ).Apply( Holder );
            State = 5;
        }
        else{ Ticks--; }
        TextHandle.WritePublicText( "Retract Piston Y 4" );
    }
    //Y Retract
    if( State == 5 )
    {
        if( float.Parse( Pistons[1].DetailedInfo.Split( ':' )[1].Split( 'm' )[0] ) < Pistons[1].GetValue<float>( "LowerLimit" ) + 0.01f )
        {
            ProjectorHandle.GetActionWithName( "OnOff_Off" ).Apply( ProjectorHandle );
            if( Stop )
            {
                State = 0;
                Stop = false;
            }
            else
            {
                State = 1;
                Pistons[0].GetActionWithName( "Extend" ).Apply( Pistons[0] );
            }
        }
        TextHandle.WritePublicText( "Retract Piston Y 5" );
    }

}

//X 7 Max