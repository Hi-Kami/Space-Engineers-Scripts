IMyCockpit MainCockpit;
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
    MainCockpit = GridTerminalSystem.GetBlockWithName( "Cockpit" ) as IMyCockpit;
    Me.GetSurface(0).WriteText(MainCockpit.SurfaceCount.ToString());
    for( int i = 0; i < MainCockpit.SurfaceCount; i++ )
    {
        MainCockpit.GetSurface(i).ContentType = ContentType.TEXT_AND_IMAGE; 
    }
}

public void Save()
{

}

public void Main( string argument, UpdateType updateSource )
{
    for( int i = 0; i < MainCockpit.SurfaceCount; i++ )
    {
        MainCockpit.GetSurface(i).WriteText( i.ToString() );
    }
}

