"""
A collection of models we'll use to attempt to classify videos.
"""
from keras.layers.normalization import BatchNormalization
from keras.layers import Dense, Flatten, Dropout, Embedding
from keras.layers.recurrent import LSTM
from keras.regularizers import l2
from keras.models import Sequential, load_model
from keras.layers import MaxPooling2D, TimeDistributed 
from keras.layers import Dense, Dropout, Activation, Flatten, Bidirectional
from keras.optimizers import Adam
from keras.layers.wrappers import TimeDistributed
from keras.layers.convolutional import (Conv2D, MaxPooling3D, Conv3D,
    MaxPooling2D)
from collections import deque
import sys
from keras.optimizers import SGD
#from phased_lstm_keras.PhasedLSTM import PhasedLSTM

class ResearchModels():
    def __init__(self, nb_classes, model, seq_length,
                 saved_model=None, features_length=2048):
        """
        `model` = one of:
            lstm
            crnn
            mlp
            conv_3d
        `nb_classes` = the number of classes to predict
        `seq_length` = the length of our video sequences
        `saved_model` = the path to a saved Keras model to load
        """

        # Set defaults.
        self.seq_length = seq_length
        self.load_model = load_model
        self.saved_model = saved_model
        self.nb_classes = nb_classes
        self.feature_queue = deque()

        # Set the metrics. Only use top k if there's a need.
        metrics = ['accuracy']
        if self.nb_classes >= 10:
            metrics.append('top_k_categorical_accuracy')

        # Get the appropriate model.
        if self.saved_model is not None:
            print("Loading model %s" % self.saved_model)
            self.model = load_model(self.saved_model)
        elif model == 'cnn_rnn':
            print("Loading cnn_rnn model.")
            self.input_shape = (seq_length, features_length)
            self.model = self.cnn_rnn()
        elif model == 'lstm':
            print("Loading LSTM model.")
            self.input_shape = (seq_length, features_length)
            self.model = self.lstm()
        elif model == 'crnn':
            print("Loading CRNN model.")
            self.input_shape = (seq_length, 80, 80, 3)
            self.model = self.crnn()
        elif model == 'mlp':
            print("Loading simple MLP.")
            self.input_shape = features_length * seq_length
            self.model = self.mlp()
        elif model == 'conv_3d':
            print("Loading Conv3D")
            self.input_shape = (seq_length, 80, 80, 3)
            self.model = self.conv_3d()
        else:
            print("Unknown network.")
            sys.exit()

        #sgd = SGD(lr=0.00005, decay = 1e-6, momentum=0.9, nesterov=True)
        #self.model.compile(optimizer=sgd, loss='categorical_crossentropy', metrics=['accuracy'])
        
        
        optimizer = Adam(lr=1e-6)  # aggressively small learning rate
        self.model.compile(loss='categorical_crossentropy', optimizer=optimizer,
                           metrics=metrics)
        
        
        
    def cnn_rnn(self):
        model = Sequential()
        model.add(LSTM(256,dropout=0.2,input_shape=self.inputshape))
        model.add(Dense(1024, activation='relu'))
        model.add(Dropout(0.5))
        model.add(Dense(5, activation='softmax'))
        return model

    def lstm(self):
        """Build a simple LSTM network. We pass the extracted features from
        our CNN to this model predomenently."""
       #  Model.
#        model = Sequential()
#        model.add(LSTM(2048, return_sequences=True, input_shape=self.input_shape,
#                       dropout=0.5))
#        model.add(Flatten())
#        model.add(Dense(512, activation='relu'))
#        model.add(BatchNormalization())
#        model.add(TimeDistributed(Flatten()))
#        model.add(LSTM(128, return_sequences= True,
#                       dropout=0.5))
#        model.add(Dense(32, activation='relu'))
#        model.add(Dropout(0.2))

        model_lstm = Sequential()
        model_lstm.add(LSTM(input_dim=2048, output_dim=100, return_sequences=True)) 
        model_lstm.add(Dropout(0.3)) 
        model_lstm.add(LSTM(input_dim=100, output_dim=20, return_sequences=False)) 
        model_lstm.add(Dense(2, W_regularizer=l2(0.06))) 
        model_lstm.add(Activation('softmax')) 
        model_lstm.add(Dense(self.nb_classes, activation='softmax'))

# 이 model 이 원래 model이얌 

        model = Sequential() 
        model.add(LSTM(2048, return_sequences=True, stateful=False, input_shape=self.input_shape)) 
        model.add(LSTM(512, return_sequences=True, stateful=False)) 
        model.add(LSTM(64)) 
        model.add(Dense(32, activation='relu')) 
        model.add(Dropout(0.5))  
        model.add(Dense(self.nb_classes, activation='softmax'))
        
        return model

        
##        
#        model_lstm2 = Sequential()
#        # Embedding layer
#        model_lstm2.add(Embedding(2048, 5,
#                        input_shape=self.input_shape))
#        # LSTM Layer #1
#        lstm = LSTM(256, return_sequences=True, unroll=True,
#                    dropout=0.1, recurrent_dropout=0.1)
#        model_lstm2.add(Bidirectional(lstm))
#        model_lstm2.add(Dropout(0.1))
#        # LSTM Layer #2
#        lstm = LSTM(256, return_sequences=True, unroll=True,
#                    dropout=0.1, recurrent_dropout=0.1)
#        model_lstm2.add(Bidirectional(lstm))
#        model_lstm2.add(Dropout(0.1))
#        # LSTM Layer #3
#        lstm = LSTM(128, return_sequences=True, unroll=True,
#                    dropout=0.25, recurrent_dropout=0.25)
#        model_lstm2.add(Bidirectional(lstm))
#        model_lstm2.add(Dropout(0.25))
#        # RNN
#        model_lstm2.add(TimeDistributed(Dense(32, activation="softmax"),
#                                  input_shape=(40, 128)))
#
#        model_lstm2.add(Dense(self.nb_classes, activation='softmax'))
        
        
        
 #s       model_lstm3 = Sequential()
#        model_lstm3.add(TimeDistributed(Conv2D(32, 3, padding='same'), input_shape=self.input_shape))
#        model_lstm3.add(TimeDistributed(Conv2D(32, 3, padding='same', activation='relu')))
#        model_lstm3.add(TimeDistributed(Conv2D(32, 3, padding='same', activation='relu')))
#        model_lstm3.add(TimeDistributed(Conv2D(32, 3, padding='same', activation='relu')))
#        model_lstm3.add(TimeDistributed(
#            MaxPooling2D(
#                pool_size=(2, 2),
#                strides=2,
#                dim_ordering="th",
#            )))
#        model_lstm3.add(TimeDistributed(Conv2D(64, 3, padding='same', activation='relu')))
#        model_lstm3.add(TimeDistributed(Conv2D(64, 3, padding='same', activation='relu')))
#        model_lstm3.add(TimeDistributed(
#            MaxPooling2D(
#                pool_size=(2, 2),
#                strides=2,
#                dim_ordering="th",
#            )))
#        model_lstm3.add(TimeDistributed(Conv2D(128, 3, padding='same', activation='relu')))
#        model_lstm3.add(TimeDistributed(
#            MaxPooling2D(
#                pool_size=(2, 2),
#                strides=2,
#                dim_ordering="th"
#            )))
#        model_lstm3.add(Dense(self.nb_classes, activation='softmax'))
        
        
#        model_lstm4 = Sequential()
#        embedding_layer = Embedding(3,
#   		input_shape=self.input_shape,
#    		trainable=False)
#        model_lstm4.add(embedding_layer)
#        model_lstm4.add(Bidirectional(LSTM(64)))
#        model_lstm4.add(Dropout(0.5))
#        model_lstm4.add(Dense(self.nb_classes, activation='softmax'))
#        
#        model_lstm5 = Sequential()
#        model_lstm5.add(PhasedLSTM(2048, return_sequences=True, input_shape=self.input_shape,dropout=0.5))
#        model_lstm5.add(Flatten())
#        model_lstm5.add(Dense(1024, activation='relu'))
#        model_lstm5.add(Dropout(0.5))
#        model_lstm5.add(Dense(self.nb_classes, activation='softmax'))
#
#        model_lstm6 = Sequential()
#        model_lstm6.add(Embedding(3, 256))
#        model_lstm6.add(LSTM(input_dim=100, output_dim=20, return_sequences=False)) 
#        model_lstm6.add(Dropout(0.5))
#        model_lstm6.add(Dense(128, init='uniform'))
#        model_lstm6.add(Dense(self.nb_classes, activation='softmax'))        
#        
    
    def crnn(self):
        """Build a CNN into RNN.
        Starting version from:
        https://github.com/udacity/self-driving-car/blob/master/
            steering-models/community-models/chauffeur/models.py
        """
#        model = Sequential()
#        model.add(TimeDistributed(Conv2D(32, (3,3),
#            kernel_initializer="he_normal",
#            activation='relu'), input_shape=self.input_shape))
#        model.add(TimeDistributed(Conv2D(32, (3,3),
#            kernel_initializer="he_normal",
#            activation='relu')))
#        model.add(TimeDistributed(MaxPooling2D()))
#        model.add(TimeDistributed(Conv2D(48, (3,3),
#            kernel_initializer="he_normal",
#            activation='relu')))
#        model.add(TimeDistributed(Conv2D(48, (3,3),
#            kernel_initializer="he_normal",
#            activation='relu')))
#        model.add(TimeDistributed(MaxPooling2D()))
#        model.add(TimeDistributed(Conv2D(64, (3,3),
#            kernel_initializer="he_normal",
#            activation='relu')))
#        model.add(TimeDistributed(Conv2D(64, (3,3),
#            kernel_initializer="he_normal",
#            activation='relu')))
#        model.add(TimeDistributed(MaxPooling2D()))
#        model.add(TimeDistributed(Conv2D(128, (3,3),
#            kernel_initializer="he_normal",
#            activation='relu')))
#        model.add(TimeDistributed(Conv2D(128, (3,3),
#            kernel_initializer="he_normal",
#            activation='relu')))
#        model.add(TimeDistributed(MaxPooling2D()))
#        model.add(TimeDistributed(Flatten()))
#        model.add(LSTM(256, return_sequences=True))
#        model.add(Flatten())
#        model.add(Dense(512))
#        model.add(Dropout(0.5))
#        model.add(Dense(self.nb_classes, activation='softmax'))
        
        
        
        
        model = Sequential() 
        model.add(TimeDistributed(Conv2D(32, 3, 3, border_mode='same'), input_shape=(40,2048), name='convolution2d_1')) 
        model.add(TimeDistributed(Activation('relu'))) 
        model.add(TimeDistributed(Conv2D(32, 3, 3),  name='convolution2d_2')) 
        model.add(TimeDistributed(Activation('relu'))) 
        model.add(TimeDistributed(MaxPooling2D(pool_size=(2, 2)))) 
        model.add(TimeDistributed(Dropout(0.25))) 
        model.add(TimeDistributed(Conv2D(64, 3, 3, border_mode='same'), name='convolution2d_3')) 
        model.add(TimeDistributed(Activation('relu'))) 
        model.add(TimeDistributed(Conv2D(64, 3, 3), name='convolution2d_4')) 
        model.add(TimeDistributed(Activation('relu'))) 
        model.add(TimeDistributed(MaxPooling2D(pool_size=(2, 2)))) 
        model.add(TimeDistributed(Dropout(0.25))) 
        model.add(TimeDistributed(Flatten())) 
        model.add(TimeDistributed(Dense(512), name='dense_1')) 
        model.add(TimeDistributed(Activation('relu'))) 
        model.add(TimeDistributed(Dropout(0.5))) 
        model.add(LSTM(512, return_sequences=True)) 
        model.add(TimeDistributed(Dense(4)))
        model.add(Dense(self.nb_classes, activation='softmax'))
        
    


        return model

    def mlp(self):
        """Build a simple MLP."""
        # Model.
        model = Sequential()
        model.add(Dense(512, input_dim=self.input_shape))
        model.add(Dropout(0.5))
        model.add(Dense(512))
        model.add(Dropout(0.5))
        model.add(Dense(self.nb_classes, activation='softmax'))

        return model

    def conv_3d(self):
        """
        Build a 3D convolutional network, based loosely on C3D.
            https://arxiv.org/pdf/1412.0767.pdf
        """
        # Model.
        model = Sequential()
        model.add(Conv3D(
            32, (7,7,7), activation='relu', input_shape=self.input_shape
        ))
        model.add(MaxPooling3D(pool_size=(1, 2, 2), strides=(1, 2, 2)))
        model.add(Conv3D(64, (3,3,3), activation='relu'))
        model.add(MaxPooling3D(pool_size=(1, 2, 2), strides=(1, 2, 2)))
        model.add(Conv3D(128, (2,2,2), activation='relu'))
        model.add(MaxPooling3D(pool_size=(1, 2, 2), strides=(1, 2, 2)))
        model.add(Flatten())
        model.add(Dense(256))
        model.add(Dropout(0.2))
        model.add(Dense(256))
        model.add(Dropout(0.2))
        model.add(Dense(self.nb_classes, activation='softmax'))

        return model
        
    


    

'''
"""
A collection of models we'll use to attempt to classify videos.
"""
from keras.layers.normalization import BatchNormalization
from keras.layers import Dense, Flatten, Dropout
from keras.layers.recurrent import LSTM
from keras.models import Sequential, load_model
from keras.optimizers import Adam
from keras.layers.wrappers import TimeDistributed
from keras.layers.convolutional import (Conv2D, MaxPooling3D, Conv3D,
    MaxPooling2D)
from collections import deque
import sys
from keras.optimizers import SGD

class ResearchModels():
    def __init__(self, nb_classes, model, seq_length,
                 saved_model=None, features_length=2048):
        """
        `model` = one of:
            lstm
            crnn
            mlp
            conv_3d
        `nb_classes` = the number of classes to predict
        `seq_length` = the length of our video sequences
        `saved_model` = the path to a saved Keras model to load
        """

        # Set defaults.
        self.seq_length = seq_length
        self.load_model = load_model
        self.saved_model = saved_model
        self.nb_classes = nb_classes
        self.feature_queue = deque()

        # Set the metrics. Only use top k if there's a need.
        metrics = ['accuracy']
        if self.nb_classes >= 10:
            metrics.append('top_k_categorical_accuracy')

        # Get the appropriate model.
        if self.saved_model is not None:
            print("Loading model %s" % self.saved_model)
            self.model = load_model(self.saved_model)
        elif model == 'cnn_rnn':
            print("Loading cnn_rnn model.")
            self.input_shape = (seq_length, features_length)
            self.model = self.cnn_rnn()
        elif model == 'lstm':
            print("Loading LSTM model.")
            self.input_shape = (seq_length, features_length)
            self.model = self.lstm()
        elif model == 'crnn':
            print("Loading CRNN model.")
            self.input_shape = (seq_length, 80, 80, 3)
            self.model = self.crnn()
        elif model == 'mlp':
            print("Loading simple MLP.")
            self.input_shape = features_length * seq_length
            self.model = self.mlp()
        elif model == 'conv_3d':
            print("Loading Conv3D")
            self.input_shape = (seq_length, 80, 80, 3)
            self.model = self.conv_3d()
        else:
            print("Unknown network.")
            sys.exit()

      optimizer = Adam(lr=1e-6)  # aggressively small learning rate
        self.model.compile(loss='categorical_crossentropy', optimizer=optimizer,
                           metrics=metrics)

    def lstm(self):
        """Build a simple LSTM network. We pass the extracted features from
        our CNN to this model predomenently."""
       #  Model.
#        model = Sequential()
#        model.add(LSTM(2048, return_sequences=True, input_shape=self.input_shape,
#                       dropout=0.5))
#        model.add(Flatten())
#        model.add(Dense(512, activation='relu'))
#        model.add(BatchNormalization())
#        model.add(TimeDistributed(Flatten()))
#        model.add(LSTM(128, return_sequences= True,
#                       dropout=0.5))
#        model.add(Dense(32, activation='relu'))
#        model.add(Dropout(0.2))
        model = Sequential() 
        model.add(LSTM(2048, return_sequences=True, stateful=False, input_shape=self.input_shape)) 
        model.add(LSTM(512, return_sequences=True, stateful=False)) 
        model.add(LSTM(64)) 
        model.add(Dense(32, activation='relu')) 
        model.add(Dropout(0.5)) 
        model.add(Dense(self.nb_classes, activation='softmax'))
        return model

    def crnn(self):
        """Build a CNN into RNN.
        Starting version from:
        https://github.com/udacity/self-driving-car/blob/master/
            steering-models/community-models/chauffeur/models.py
        """
        model = Sequential()
        model.add(TimeDistributed(Conv2D(32, (3,3),
            kernel_initializer="he_normal",
            activation='relu'), input_shape=self.input_shape))
        model.add(TimeDistributed(Conv2D(32, (3,3),
            kernel_initializer="he_normal",
            activation='relu')))
        model.add(TimeDistributed(MaxPooling2D()))
        model.add(TimeDistributed(Conv2D(48, (3,3),
            kernel_initializer="he_normal",
            activation='relu')))
        model.add(TimeDistributed(Conv2D(48, (3,3),
            kernel_initializer="he_normal",
            activation='relu')))
        model.add(TimeDistributed(MaxPooling2D()))
        model.add(TimeDistributed(Conv2D(64, (3,3),
            kernel_initializer="he_normal",
            activation='relu')))
        model.add(TimeDistributed(Conv2D(64, (3,3),
            kernel_initializer="he_normal",
            activation='relu')))
        model.add(TimeDistributed(MaxPooling2D()))
        model.add(TimeDistributed(Conv2D(128, (3,3),
            kernel_initializer="he_normal",
            activation='relu')))
        model.add(TimeDistributed(Conv2D(128, (3,3),
            kernel_initializer="he_normal",
            activation='relu')))
        model.add(TimeDistributed(MaxPooling2D()))
        model.add(TimeDistributed(Flatten()))
        model.add(LSTM(256, return_sequences=True))
        model.add(Flatten())
        model.add(Dense(512))
        model.add(Dropout(0.5))
        model.add(Dense(self.nb_classes, activation='softmax'))

        return model

    def mlp(self):
        """Build a simple MLP."""
        # Model.
        model = Sequential()
        model.add(Dense(512, input_dim=self.input_shape))
        model.add(Dropout(0.5))
        model.add(Dense(512))
        model.add(Dropout(0.5))
        model.add(Dense(self.nb_classes, activation='softmax'))

        return model

    def conv_3d(self):
        """
        Build a 3D convolutional network, based loosely on C3D.
            https://arxiv.org/pdf/1412.0767.pdf
        """
        # Model.
        model = Sequential()
        model.add(Conv3D(
            32, (7,7,7), activation='relu', input_shape=self.input_shape
        ))
        model.add(MaxPooling3D(pool_size=(1, 2, 2), strides=(1, 2, 2)))
        model.add(Conv3D(64, (3,3,3), activation='relu'))
        model.add(MaxPooling3D(pool_size=(1, 2, 2), strides=(1, 2, 2)))
        model.add(Conv3D(128, (2,2,2), activation='relu'))
        model.add(MaxPooling3D(pool_size=(1, 2, 2), strides=(1, 2, 2)))
        model.add(Flatten())
        model.add(Dense(256))
        model.add(Dropout(0.2))
        model.add(Dense(256))
        model.add(Dropout(0.2))
        model.add(Dense(self.nb_classes, activation='softmax'))

        return model
'''