Space Engineers Workshop;
Unfoundbug AutoRecharge
https://steamcommunity.com/sharedfiles/filedetails/?id=2808241923

======================

Developed by UnFoundBug.

======================

	Auto Recharge adds an option to connectors which allows all connected batteries on the grid to switch to recharge on lock.
		Static restriction only switches the batteries to recharge if the connected grid is static.
		
======================
V0.1.0:
	Multiplayer implementation.
		Dedicated server and player server support.
V0.0.4:
	Bug fix: Sometimes settings were getting lost on world load, this should be fixed now.
	Performance improvements:
		New method for storing connector configurations should mean save/load/change of connector configs is faster
V0.0.3:
	Added support for gas tank stockpiling
		Connectors can now automatically set/clear stockpiling of gasses on connection to other grids.
			None,O2,H2 and All tank filter options available.
V0.0.2:
	Fixed issue with changing configuration during gameplay
	Added thruster mode configuration
		Automatically disables and enables various thruster configurations based on connector state.
		None disables thruster control.
		Electic only controls atmospheric and Ion engines.
		Hydrogen only controls hydrogen, 
		All affects all
V0.0.1: First Implementation
	Moving Grid batteries provide Auto-Recharge Option
	All batteries provide Auto-Discharge Option
