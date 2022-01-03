extends Spatial

func _ready():
	var terrain = get_node("../")
	var _data = terrain.get("_terrain_data")
	var heightmap_size = 256
	var terrain_size = _data.get_resolution()
	var area_size = terrain_size / heightmap_size
	var MAX_HEIGHT = 100.0
	var img = Image.new()
	var heights = _data.get_all_heights()
	img.create(heightmap_size, heightmap_size, false, Image.FORMAT_RGBA8)
	img.lock()
	for i in heightmap_size:
		for j in heightmap_size:
			var r_factor = getAverageHeight(i * area_size, j * area_size, area_size, heights, terrain_size) / MAX_HEIGHT
			img.set_pixel(i, j, Color(0.58, 0.4, 0.23, r_factor))
	img.unlock()
	img.save_png("Resources/Terrains/Terrain1/heightmap.png")


func getAverageHeight(start_x, start_y, square_side, data, terrain_size) -> float:
	var total_height = 0.0
	for i in square_side:
		for j in square_side:
			total_height += data[terrain_size * (start_y + i) + start_x + j]
	
	return total_height / (square_side * square_side)
