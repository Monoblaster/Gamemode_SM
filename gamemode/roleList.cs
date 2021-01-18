function MinigameSO::makeRoleList(%mini, %numPlayers, %teamObjectField){
	//set up a new object to store the role list array to make it easy to pass arround
	if(isObject(%mini.roleList)){
		%mini.roleList.delete();
	}
	%roleList = new scriptObject(){
		class = "roleList";
		roleCount = 0;
	};
	%mini.roleList = %roleList;
	//fill a local array with the teams and also calculate to total weight between the teams
	%teamLength = getFieldCount(%teamObjectField);
	%totalTeamWeight = 0;
	for(%i = 0; %i < %teamLength; %i++){
		%team = getField(%teamObjectField, %i);
		%totalTeamWeight += %team.weightValue;
		%team[%i] = %team;
	}
	//get the team percent weight for each team compared to total weight
	//then set the number of players based on the weight
	%mini.currentTeamCount = 0;
	%playerCount = 0;
	%currentWeight = %playerCount / %numPlayers;
	%teamWeight = 0;
	for(%i = 0; %i < %teamLength; %i++){
		%team = getField(%teamObjectField, %i);
		//add previous team's weight so this works
		%teamWeight = %team.weightValue / %totalTeamWeight + %teamWeight;
		%teamPlayers[%i] = 0;
		while(%playerCount < %numPlayers && %currentWeight <= %teamWeight){
			%teamPlayers[%i]++;
			%playerCount++;
			%currentWeight = %playerCount / %numPlayers;
		}
		//sets up win conditions and stuff for winning
		if(%teamPlayers[%i] > 0){
			%mini.currentTeam[%mini.currentTeamCount] = %team.getName();
			%mini.currentTeamCount++;
		}
		//for when assigning roles to each player;
		%assignedPlayers[%i] = 0;
		%teamValue[%i] = 0;
	}
	//we know want to make our balanced teams
	%totalAssignedPlayers = 0;
	//we loop through every player until they all have a role
	while(%totalAssignedPlayers < %numPlayers){
		//for each team we want to choose a role if they haven't already chosen all of them
		for(%i = 0; %i < %teamLength && %totalAssignedPlayers < %numPlayers; %i++){
			if(%assignedPlayers[%i] < %teamPlayers[%i]){
				%team = %team[%i];
				//to choose a role we follow these rules
				//a role must always be chosen on a turn
				//you must include roles with minmum not satisfied yet
				//you must not include roles which have already hit a maximum
				//you must always as high value of a role as you can
				//you must not excede the other player's points if possible
				
				//first we check if there are any unsatisfied minimums
				%continue = false;
				for(%j = 0; %j < %team.roleNum; %j++){
					%role = %team.role[%j];
					//role_Sky_Marshal should how the count if we were testing the Sky_Marshal role
					if(%role.min > %roleCount[%role.getName() @ %team.getName()] && %role.min != -1){
						%roleList.addRole(%role);
						%teamValue[%i]+= %role.pointValue;
						%assignedPlayers[%i]++;
						%totalAssignedPlayers++;
						%roleCount[%role.getName() @ %team.getName()]++;
						//stop so we do this turn by turn
						%continue = true;
						break;
					}
				}
				if(%continue)
					continue;
				%min = inf;
				//we want to calculate what the current lowest score out of the other teams is
				for(%j = 0; %j < %teamLength; %j++){
					if(%team.getId() == %team[%j].getId()){
						//we don't want to count our own team;
						continue;
					}
					if(%teamValue[%j] < %min){
						%min = %teamValue[%j];
					}
				}
				
				//we can use min to figure out how many points are availible
				%availiblePoints = %min - %teamValue[%i];
				//next we choose a role to add based on the other team's value and our selection
				%chosenRole = 0;
				for(%j = 0; %j < %team.roleNum; %j++){
					%role = %team.role[%j];
					%rolecost = %availiblePoints - %role.pointValue;
					if(%roleCount[%role.getName()] >= %role.max && %role.max != -1){
						//this role hit the max so we want to continue
						continue;
					}
					//if the cost is less than 0 we want to look for the lowest possible value
					if(%roleCost <= 0){
						if(%role.pointValue < %chosenrole.pointValue || !isObject(%chosenrole)){
							%chosenRole = %role;
						}
					} else{
					//else we look for the highest possible value
						if(%role.pointValue > %chosenRole.pointValue){
							%chosenRole = %role;
						}
					}
				}
				//we add the role in
				%roleList.addRole(%chosenRole);
				%teamValue[%i]+= %chosenRole.pointValue;
				%assignedPlayers[%i]++;
				%totalAssignedPlayers++;
				%roleCount[%chosenRole.getName() @ %team.getName()]++;
			}
		}
	}
	//we then randomize roles to make meta gaming impossible
	%roleList.randomizeRoleList();
}
//does most of the work when adding roles
function roleList::addRole(%list, %role){
	%list.role[%list.roleCount] = %role;
	%list.roleCount++;
}
//randomizes the list for the final product
function roleList::randomizeRoleList(%list){
	%numPlayers = %list.roleCount;
	for(%i = %numplayers - 1; %i > 0; %i--){
		%r = getRandom(0, %i);
		%temp = %list.role[%i];
		%list.role[%i] = %list.role[%r];
		%list.role[%r] = %temp;
	}
}
