fx_version 'cerulean'
game 'gta5'

author 'Roxstar Studios'
developers 'XdGoldenTiger'
description 'RoxRemoteVehicle Resource'
version '1.0.0'

clr_disable_task_scheduler 'yes'

client_scripts {
	'RoxRemoteVehicle.Client.net.dll'
}

server_scripts {
	'RoxRemoteVehicle.Server.net.dll'
}

files {
	'*.dll',
	'data/*',
	'stream/*',
	'Config.ini',
}

data_file 'HANDLING_FILE' 'data/handling.meta'
data_file 'VEHICLE_METADATA_FILE' 'data/vehicles.meta'
data_file 'CARCOLS_FILE' 'data/carcols.meta'
data_file 'VEHICLE_VARIATION_FILE' 'data/carvariations.meta'
data_file 'VEHICLE_LAYOUTS_FILE' 'data/vehiclelayouts.meta'
