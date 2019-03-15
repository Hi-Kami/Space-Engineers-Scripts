//Componants and vars 
 
List<IMyPistonBase> PistonHandleList; 
List<IMyShipDrill> DrillHandleList; 
IMyTextPanel TextHandle; 
 
float CurrentPistonLength = 0.0f; 
int CurrentPistonNumber = 1; 
string OutputText = ""; 
 
byte DrillState = 0;//0 = Off, 1 = Drill, 2 = Return, 3 = Finished and return 
 
public Program() 
{ 
	//Set to loop 
	Runtime.UpdateFrequency = UpdateFrequency.Update1;  
	 
	//Get handle to output text lcd 
	TextHandle = GridTerminalSystem.GetBlockWithName( "(o)" ) as IMyTextPanel; 
	 
	//Loop Stopper 
	bool StopLoop = true; 
		 
	//Get Pistons 
	PistonHandleList = new List<IMyPistonBase>();		 
	do 
	{ 
		IMyPistonBase tmp = GridTerminalSystem.GetBlockWithName( "(p)" + CurrentPistonNumber ) as IMyPistonBase; 
		if( tmp == null ){ StopLoop = false; } 
		else 
		{ 
			PistonHandleList.Add( tmp ); 
			CurrentPistonNumber++; 
		} 
	} 
	while( StopLoop ); 
	 
	StopLoop = true; 
	CurrentPistonNumber -= 2; 
		 
	//Get Drills 
	DrillHandleList = new List<IMyShipDrill>();	 
	int TmpNum = 1; 
	do 
	{ 
		IMyShipDrill tmp = GridTerminalSystem.GetBlockWithName( "(d)" + TmpNum  ) as IMyShipDrill; 
		if( tmp == null ){ StopLoop = false; } 
		else 
		{ 
			DrillHandleList.Add( tmp ); 
			TmpNum++; 
		} 
	} 
	while( StopLoop ); 
} 
 
 
 
public void Save() 
{ 
} 
 
 
 
public void Main( string argument, UpdateType updateSource ) 
{ 
        //Input: Drill, Return 
		int TmpVar; 
        switch( argument )     
        { 
			case "Drill": 
				DrillState = 1; 
				TmpVar = DrillHandleList.Count; 
				for( byte i = 0; i < TmpVar; i++ ) 
				{ 
					DrillHandleList[i].GetActionWithName( "OnOff_On" ).Apply( DrillHandleList[i] ); 
				} 
				PistonHandleList[CurrentPistonNumber].GetActionWithName( "OnOff_On" ).Apply( PistonHandleList[CurrentPistonNumber] ); 
				PistonHandleList[CurrentPistonNumber].GetActionWithName( "Extend" ).Apply( PistonHandleList[CurrentPistonNumber] ); 
			break; 
			 
			case "Return": 
				DrillState = 2; 
				TmpVar = DrillHandleList.Count; 
				for( byte i = 0; i < TmpVar; i++ ) 
				{ 
					DrillHandleList[i].GetActionWithName( "OnOff_Off" ).Apply( DrillHandleList[i] ); 
				} 
				PistonHandleList[CurrentPistonNumber].GetActionWithName( "OnOff_On" ).Apply( PistonHandleList[CurrentPistonNumber] ); 
				PistonHandleList[CurrentPistonNumber].GetActionWithName( "Retract" ).Apply( PistonHandleList[CurrentPistonNumber] ); 
				Retract(); 
			break; 
			 
			default: 
				CurrentPistonLength = float.Parse( PistonHandleList[CurrentPistonNumber].DetailedInfo.Split( ':' )[1].Split( 'm' )[0] ); 
				if( DrillState == 0 ) 
				{ 
					OutputText = "Drill Mode: Off \n"; 
				} 
				else if( DrillState == 1 ) 
				{ 
					OutputText = "Drill Mode: Drilling \n"; 
					OutputText += "Piston: " + CurrentPistonNumber + " \n"; 
					OutputText += "Piston Length: " + CurrentPistonLength + " \n"; 
					//if piston fully extended 
					if( CurrentPistonLength > 9.9f ) 
					{ 
						//If out of pistons 
						if( CurrentPistonNumber == 0 ) 
						{ 
							DrillState = 3;//Finished 
							PistonHandleList[CurrentPistonNumber].GetActionWithName( "Retract" ).Apply( PistonHandleList[CurrentPistonNumber] ); 
							TmpVar = DrillHandleList.Count; 
							for( byte i = 0; i < TmpVar; i++ ) 
							{ 
								DrillHandleList[i].GetActionWithName( "OnOff_Off" ).Apply( DrillHandleList[i] ); 
							} 
						} 
						else 
						{ 
							PistonHandleList[CurrentPistonNumber].GetActionWithName( "OnOff_Off" ).Apply( PistonHandleList[CurrentPistonNumber] ); 
							CurrentPistonLength = 0.0f; 
							CurrentPistonNumber--; 
							PistonHandleList[CurrentPistonNumber].GetActionWithName( "OnOff_On" ).Apply( PistonHandleList[CurrentPistonNumber] ); 
							PistonHandleList[CurrentPistonNumber].GetActionWithName( "Extend" ).Apply( PistonHandleList[CurrentPistonNumber] ); 
						} 
					} 
				} 
				else if( DrillState == 2 ) 
				{ 
					OutputText = "Drill Mode: Returning \n"; 
					OutputText += "Piston: " + CurrentPistonNumber + " \n"; 
					OutputText += "Piston Length: " + CurrentPistonLength + " \n"; 
					Retract(); 
				} 
				else if( DrillState == 3 ) 
				{ 
					OutputText = "Drill Mode: Finished, Returning Drill \n"; 
					OutputText += "Piston: " + CurrentPistonNumber + " \n"; 
					OutputText += "Piston Length: " + CurrentPistonLength + " \n"; 
					Retract(); 
				} 
			break; 
        } 
		 
		CurrentPistonLength = float.Parse( PistonHandleList[CurrentPistonNumber].DetailedInfo.Split( ':' )[1].Split( 'm' )[0] ); 
		 
		//Output text if exsists 
		if(TextHandle != null ) 
		{ 
			TextHandle.WritePublicText( OutputText ); 
		} 
} 
 
public void Retract() 
{ 
	//if piston fully retracted 
	if( CurrentPistonLength < 0.1f ) 
	{ 
		//If out of pistons 
		if( CurrentPistonNumber > PistonHandleList.Count - 2 )  
		{ 
			DrillState = 0;//Finished 
			PistonHandleList[CurrentPistonNumber].GetActionWithName( "OnOff_Off" ).Apply( PistonHandleList[CurrentPistonNumber] ); 
			PistonHandleList[CurrentPistonNumber].GetActionWithName( "Extend" ).Apply( PistonHandleList[CurrentPistonNumber] ); 
		} 
		else 
		{ 
			PistonHandleList[CurrentPistonNumber].GetActionWithName( "OnOff_Off" ).Apply( PistonHandleList[CurrentPistonNumber] ); 
			PistonHandleList[CurrentPistonNumber].GetActionWithName( "Extend" ).Apply( PistonHandleList[CurrentPistonNumber] ); 
			CurrentPistonNumber++; 
			CurrentPistonLength = 10.0f; 
			PistonHandleList[CurrentPistonNumber].GetActionWithName( "OnOff_On" ).Apply( PistonHandleList[CurrentPistonNumber] ); 
			PistonHandleList[CurrentPistonNumber].GetActionWithName( "Retract" ).Apply( PistonHandleList[CurrentPistonNumber] ); 
		} 
	} 
} 
 
