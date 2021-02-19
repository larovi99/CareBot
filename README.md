# CareBot
Carebot is a robotic project focused on helping the elderly and mobility reduced people in their daily tasks, such as grabbing things that are out of range, have fallen to the ground, or moving objects too heavy for them. 

## Installation
Python requirements
- opencv
- imageia
- keras
- tensorflow 1.14

It is also necessary to get the yolo.h5 neural networks file, which couldn't be uploaded to github due to its big size. You can get it [here](https://drive.google.com/open?id=1cAPrnHMpSoOYay5PKQjfn5h6qHLlhf_f)

Once downloaded, it should be put in a folder called "models" in the Python folder.

## Simulation usage
Before running the simulation, be sure to comply with the python requirements, and set the variable pythonSource in the file PythonExecuter.cs to the path to the python executable in your system. Put the yolo neural network file under the models folder in the python folder of the simulation project.