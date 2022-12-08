extends Label

func _ready():
	self.set_counter()

func set_counter():
	self.set_text("FPS: " + str(Engine.get_frames_per_second()));

func _physics_process(_delta: float):
	self.set_counter() 
