

from imageai.Detection import ObjectDetection
import cv2
import numpy as np

"""
Object Recognition
    Model: Yolov5
    Principal library: imageai --> box points: x1, y1, x2, y2

Distances
Distances between the center of the image and the corners de bounding box are
calculate, folling the next reference:
    
    corner1        corner2
    -------------------
    |                 |
    |                 |
    |                 |
    |                 |
    |                 |
    |                 |
    -------------------
    corner3        corner4



"""




"""
# Uses the web cam to take a picture

# Opens the web cam
cap = cv2.VideoCapture(0)

ret, frame = cap.read()

if ret == True:
	cv2.imwrite("./input/input2.jpg", frame)
	print("Photo taken correctly")
else:
	print("Error to acces the camera")
  
cap.release()
cv2.destroyAllWindows()

"""
detector = ObjectDetection()

model_path = "Assets/Sources/Scripts/Python/models/yolo.h5"
input_path = "Assets/Sources/Scripts/Python/cameraImage.png"
output_path = "Assets/Sources/Scripts/Python/output/cameraImage.jpg"

detector.setModelTypeAsYOLOv3()
detector.setModelPath(model_path)
detector.loadModel()
detection = detector.detectObjectsFromImage(input_image=input_path, output_image_path=output_path)

originalImage = cv2.imread(input_path)

#Get the central pixel of the image
height, widht, color = originalImage.shape

centre_X = widht/2
centre_Y = height/2

for eachItem in detection:
    corner1_X = eachItem["box_points"][0] - centre_X
    corner1_Y = eachItem["box_points"][1] - centre_Y
    corner2_X = eachItem["box_points"][2] - centre_X
    corner2_Y = eachItem["box_points"][1] - centre_Y
    corner3_X = eachItem["box_points"][0] - centre_X
    corner3_Y = eachItem["box_points"][3] - centre_Y
    corner4_X = eachItem["box_points"][2] - centre_X
    corner4_Y = eachItem["box_points"][3] - centre_Y    
    print(eachItem["name"]+","+str(corner1_X)+","+str( corner1_Y)+","+str( corner2_X)+","+str( corner2_Y)+","+str( corner3_X)+","+str( corner3_Y)+","+str( corner4_X)+","+str( corner4_Y))