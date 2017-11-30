"""
Validate our RNN. Basically just runs a validation generator on
about the same number of videos as we have in our test set.
"""
from keras.callbacks import TensorBoard, ModelCheckpoint, CSVLogger
from models import ResearchModels
from predict_data import Predict_DataSet
import csv

import os
os.environ['TF_CPP_MIN_LOG_LEVEL']='2'

def predict(data_type, model, seq_length=80, saved_model=None,
             concat=False, class_limit=None, image_shape=None):
    batch_size = 48

    correct = 0
    
    
    # Get the data and process it.
    data = Predict_DataSet(seq_length=seq_length,
            class_limit=class_limit)   
    
    total = len(data.data)
    
    # Get the model.
    rm = ResearchModels(len(data.classes), model, seq_length, saved_model)
    
    
    for each_data in range(len(data.data)):
    
        val_generator = data.frame_generator(batch_size, each_data , data_type, concat)
    
        # predict!
        results = rm.model.predict_generator(val_generator, steps = 1)
        
        
        with open('data_file_170825_test.csv', 'w', newline='') as fout:
            writer = csv.writer(fout)
            writer.writerows(results)
    
        predict_list=[]    
        
        for row in results:
            max_idx = 0
            for i in range(len(data.classes)):
                if row[i]>row[max_idx] :
                    max_idx = i        
            predict_list.append(max_idx)
        
        predict_result = 0
        for i in range(len(data.classes)):
            if predict_list.count(i)>predict_list.count(predict_result):
                predict_result = i
        
        #predict result
        predict_class = data.classes[predict_result]
        o_or_x = ''
        
        if data.data[each_data][0].find(predict_class) != -1:
            correct = correct+1
            o_or_x = 'o'
        else:
            o_or_x = 'x'  
        
         
        print(data.data[each_data][1] + ' -> '+ predict_class+ ' : '+o_or_x)       
        
         
    
    print('correct: '+str(correct) + '/'+ str(total)+ ' ('+str(100*correct/total)+'%)')       



def main():
    model = 'lstm'
    saved_model = 'data\\checkpoints\\lstm-features.869-0.129_63.hdf5'

    data_type = 'features'
    image_shape = None
    concat = False
    
    predict(data_type, model, saved_model=saved_model,
             concat=concat, image_shape=image_shape)

if __name__ == '__main__':
    main()
