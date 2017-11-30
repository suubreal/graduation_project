# -*- coding: utf-8 -*-
"""
Created on Sat Sep 23 03:52:59 2017

@author: jisu2
"""

#!/usr/bin/python
#from _threading import *
import _thread
from struct import pack, unpack
import socket, sys, string
import predict as Predict
#import predict_thread as Predict_T
import csv
import os
import youtube_list as YoutubeList



# Start threading
def recvThread(conn):
    
    while True:
        #size만큼 받아서 메시지처리
        print('---------------------------')
        data = conn.recv(4)
        size, = unpack('<i', data)
        
        if size > 0 :
            stringdata = (conn.recv(size)).decode()
            print('> recv : ' ,stringdata)
            setAction(stringdata, conn)
       
	# If the While loop is completed, close the connection.
    conn.close()
 
def recvFile(conn):
    
    frame_count = 0
    os.system('cd C:\\Users\\MBM\\Desktop\\TRAIN\\video_classification\\data\\predict')
    os.system('cd C:\\Users\\\MBM\\Desktop\\지슈의시도\\kinectimage')
    os.system('del *.jpg')
    
    while True :
        print('>in recv file')
        data = conn.recv(4)
        size, = unpack('<i', data)
        #print('size: ', size)
    
        # /save 확인하는 부분
        if size > 0 :
            count = 0
            if size == 5 :
                count = 5
                undecoded_content = conn.recv(5)
                content = undecoded_content
                if undecoded_content.decode() == '/save':
                    print('>>>>>>>>>>>>>> break')
                    datafile = []
                    datafile.append(['predict', 'image', frame_count])

                    with open('predict_data_file.csv', 'w', newline='') as fout:
                        writer = csv.writer(fout)
                        writer.writerows(datafile)
                    break;
                
            content = conn.recv(size-count)
            with open('.\\data\\predict\\image-' + '{0:05d}'.format(frame_count) + '.jpg', 'wb') as f:
                frame_count = frame_count + 1
                print ('file opened')
                print('receiving file in...')
                f.write(content)
                f.close()
                print('Successfully get the file')
                
                
    print('end recv file!!!')
'''
    data = conn.recv(4)
    size, = unpack('<i', data)
    print('size: ', size)

    # /save 확인하는 부분 필요
    if size > 0 :
        content = conn.recv(size)
    
        with open('received_file.png', 'wb') as f:
            print ('file opened')
            print('receiving file in...')
            f.write(content)
    
            f.close()
            print('Successfully get the file')
     '''     
       
         
    
def setFileRecv(conn) :
    #file save
    print('file receiving')
    recvFile(conn);

def setPredictWindow(conn) :
    print('predictwindow!!!!')
    
    #title, id, url, channel, duration, viewcount 
    #predict 후  운동이름, 영상리스트 순서대로 보냄.
    exercise_name = Predict.main()
    conn.sendall(pack('i', len(exercise_name.encode('utf-8')))+ exercise_name.encode('utf-8'))

    setSelectWindow(exercise_name, conn)
    
    os.system('cd C:\\Users\\MBM\\Desktop\\TRAIN\\video_classification\\data\\predict')
    os.system('del *.jpg')
   
    
def setSelectWindow(ex, conn) :
    print('selectwindow!')
    
    #main에 인자로 검색할거 넣어주면된당!
    listdata = YoutubeList.main(ex);
    for setdata in listdata :
        for data in setdata :
            conn.sendall(pack('i', len(data.encode('utf-8')))+ data.encode('utf-8'))
    
    data = "/select"
    conn.sendall(pack('i', len(data.encode('utf-8')))+ data.encode('utf-8'))
    '''
#for test
def setSelectWindow(conn) :
    print('selectwindow!')
    
    #main에 인자로 검색할거 넣어주면된당!
    listdata = YoutubeList.main("Lunges");
    for setdata in listdata :
        for data in setdata :
        #print(setdata)
            conn.sendall(pack('i', len(data.encode('utf-8')))+ data.encode('utf-8'))
    
    data = "/select"
    conn.sendall(pack('i', len(data.encode('utf-8')))+ data.encode('utf-8'))
    '''
def setAction(str,conn):
    #각 스트링마다 액션 취함 > 아마 스레드처리로 변경해야 할 듯...
    switch_case = {
        "save"   :   setFileRecv,   
        "predict"   :   setPredictWindow,   
      #  "select" : setSelectWindow,
    }
    try :
        result = switch_case.get(str)(conn)
    except KeyError :
        result = 0

    
def multiServer() : 
    print ("##################################")
    print ("#     Python Network Monitor     #")
    print ("#         Server Script          #")
    print ("##################################" )
    print
    
    # This code will set the host and the port
    host = '127.0.0.1'
    port = 7001
    clients_allowed = 5
    logging_enabled = False
    
    print ( '# Server:', host, 'Port:', port, 'Allowed clients:', clients_allowed)
    
    
    try:
        # Bind socket
        print ('Binding socket...')
        s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        s.bind((host,port))
        s.listen(clients_allowed)
        
        print ('Socket binded.')
        print ('Waiting for clients to connect...')
     
        # 새로운 클라이언트가 들어올 때마다 각 스레드 할당
        while 1:
            conn, addr = s.accept()
            print ('New client connected:' + addr[0] + ':' + str(addr[1]))
            if logging_enabled == True:
                print ( 'Logfile created')
            print ('Starting to receive data...')
            _thread.start_new_thread(recvThread ,(conn,))
    
    except socket.error:
        print
        print ('Failed to bind socket, socket closed.')
        s.close
        sys.exit()
    except KeyboardInterrupt:
        print
        print ( 'Script stopped by user,','socket closed.')
        s.close
        sys.exit
    
    conn.close
    s.close()
    print ('Script finished, connection closed.')

multiServer()


