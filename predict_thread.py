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

load_model = ''
rm = 

def predict(data_type, model, seq_length=80, saved_model=None,
             concat=False, class_limit=None, image_shape=None):
    batch_size = 48

    # Get the data and process it.
    data = Predict_DataSet(seq_length=seq_length,
            class_limit=class_limit)   

    val_generator = data.frame_generator(batch_size, 'predict', data_type, concat)

    # Get the model.
    rm = ResearchModels(len(data.classes), model, seq_length, saved_model)

    # predict!
    results = rm.model.predict_generator(val_generator, steps = 1)
    
    
    with open('data_file_170924_test.csv', 'w', newline='') as fout:
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
    print(predict_result)             
    print(predict_class)
    return predict_class

def get_predict_class():
   
    #concat = True
    
    return predict(data_type, model, saved_model=saved_model,
             concat=concat, image_shape=image_shape)

def load_model():
    model = 'lstm'
    saved_model = 'data\\checkpoints\\lstm-features.546-0.195.hdf5'
    
    data_type = 'features'
    image_shape = None
    concat = False
    
    rm = ResearchModels(len(data.classes), model, seq_length, saved_model)    

'''    
    
if __name__ == '__main__':
    main()
'''