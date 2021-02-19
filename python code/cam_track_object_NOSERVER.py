import cv2
#import os
img=cv2.imread('Assets/Sources/Scripts/Python/cameraImage.png')
#print(os.getcwd(),os.getcwd())

colorLower = (33,20,40)
colorUpper = (102,255,255)
if type(img) != type(None):
    #print("Image received")
    if img.shape[0]>0 and img.shape[1]>0:
        #print("image shape")
        hsv = cv2.cvtColor(img, cv2.COLOR_BGR2HSV)
        mask = cv2.inRange(hsv, colorLower, colorUpper)
        mask = cv2.erode(mask, None, iterations=2)
        mask = cv2.dilate(mask, None, iterations=2)
        cnts = cv2.findContours(mask.copy(), cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE)[-2]
        center = None

        if len(cnts) > 0:
            c = max(cnts, key=cv2.contourArea)
            ((x, y), radius) = cv2.minEnclosingCircle(c)
            if radius < min(img.shape[0]/4,img.shape[1]/4):
                M = cv2.moments(c)
                x = int(M["m10"] / M["m00"])
                y = int(M["m01"] / M["m00"])
                center = (x, y)
                
                if x > int(2*img.shape[1]/3):
                    r = True
                    l = False
                    print("DRETA")
                elif x < int(img.shape[1]/3):
                    l = True
                    r = False
                    print("ESQUERRA")
                else:
                    l = False
                    r = False
                    print("ENDAVANT")
            else:
                print("STOP")
        else:
            print("STOP")
    else:
        print("Image shape is 0")
else:
    print("Image is none")