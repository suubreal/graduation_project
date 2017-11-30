"""
Class for managing our data.
"""
import csv
import numpy as np
import random
import glob
import os.path
import pandas as pd
import sys
import operator
from processor import process_image
from keras.utils import np_utils

class Predict_DataSet():

    def __init__(self, seq_length=80, class_limit=None, image_shape=(224, 224, 3)):
        """Constructor.
        seq_length = (int) the number of frames to consider
        class_limit = (int) number of classes to limit the data to.
            None = no limit.
        """
        self.seq_length = seq_length
        self.class_limit = class_limit
        self.sequence_path = '.\\data\\sequences\\'
        self.max_frames = 600  # max number of frames a video can have for us to use it

        # Get the data.
        self.data = self.get_data()

        # Get the classes.
        self.classes = self.get_classes()

        # Now do some minor data cleaning.
        self.data = self.clean_data()

        self.image_shape = image_shape

    #여기를 test안에 있는 것만 불러오도록 해야함
    @staticmethod
    def get_data():
        """Load our data from file."""
        print('get_data')
        with open('.\\data\\predict_data_file.csv', 'r') as fin:
            reader = csv.reader(fin)            
            data = list(reader)
            print(data)

        return data

    def clean_data(self):
        """Limit samples to greater than the sequence length and fewer
        than N frames. Also limit it to classes we want to use."""
        print('clean_data')
        data_clean = []
        for item in self.data:
            if int(item[2]) >= self.seq_length and int(item[2]) <= self.max_frames :
                data_clean.append(item)
        
        print(data_clean)
        return data_clean
    
    def get_classes(self):
        """Extract the classes from our data. If we want to limit them,
        only return the classes we need."""
        print('get_classes')
        classes = ['JumpingJack','Lunges', 'ShoulderPress', 'SideLunge', 'SidePlank', 'Squat', 'StandingPikeCrunch', 'StandingVRaise']
        
        # Return.
        if self.class_limit is not None:
            return classes[:self.class_limit]
        else:
            return classes

    def get_class_one_hot(self, class_str):
        """Given a class as a string, return its number in the classes
        list. This lets us encode and one-hot it for training."""
        # Encode it first.
        label_encoded = self.classes.index(class_str)

        # Now one-hot it.
        label_hot = np_utils.to_categorical(label_encoded, len(self.classes))
        label_hot = label_hot[0]  # just get a single row

        return label_hot

    
    def get_all_sequences_in_memory(self, batch_Size, train_test, data_type, concat=False):
        """
        This is a mirror of our generator, but attempts to load everything into
        memory so we can train way faster.
        """
        # Get the right dataset.       
        data = self.data

        print("Getting %s data with %d samples." % (train_test, len(data)))

        X = []
        for row in data:

            sequence = self.get_extracted_sequence(data_type, row)

            if sequence is None:
                print("Can't find sequence. Did you generate them?")
                raise

            if concat:
                # We want to pass the sequence back as a single array. This
                # is used to pass into a CNN or MLP, rather than an RNN.
                sequence = np.concatenate(sequence).ravel()

            X.append(sequence)            

        return np.array(X)

    def frame_generator(self, batch_size, number, data_type, concat=False):
        """Return a generator that we can use to train on. There are
        a couple different things we can return:

        data_type: 'features', 'images'
        """
        # Get the right dataset for the generator.
        data = self.data

        #print("Creating %s generator with %d samples." % (number, len(data)))

        while 1:
            X = []

            # Generate batch_size samples.
            for _ in range(batch_size):
                # Reset to be safe.
                sequence = None

                # Get a random sample.
                sample = data[number]
                #sample =random.choice(data)

                # Check to see if we've already saved this sequence.
                # Get the sequence from disk.
                sequence = self.get_extracted_sequence(data_type, sample)

                if sequence is None:
                    print("Can't find sequence. Did you generate them?")
                    sys.exit()  # TODO this should raise

                if concat:
                    # We want to pass the sequence back as a single array. This
                    # is used to pass into an MLP rather than an RNN.
                    sequence = np.concatenate(sequence).ravel()

                X.append(sequence)
                

            yield np.array(X)

    def build_image_sequence(self, frames):
        """Given a set of frames (filenames), build our sequence."""
        return [process_image(x, self.image_shape) for x in frames]

    def get_extracted_sequence(self, data_type, sample):
        """Get the saved extracted features."""
        filename = sample[1]
        path = self.sequence_path + filename + '-' + str(self.seq_length) + \
            '-' + data_type + '.txt'
            
        if os.path.isfile(path):
            # Use a dataframe/read_csv for speed increase over numpy.
            features = pd.read_csv(path, sep=" ", header=None)
            return features.values
        else:
            return None

    @staticmethod
    def get_frames_for_sample(sample):
        """Given a sample row from the data file, get all the corresponding frame
        filenames."""
        path = '.\\data\\predict\\' + sample[0] + '\\'
        filename = sample[1]
        images = sorted(glob.glob(path + filename + '*jpg'))
        return images

    @staticmethod
    def get_filename_from_image(filename):
        parts = filename.split('\\')
        return parts[-1].replace('.jpg', '')

    @staticmethod
    def rescale_list(input_list, size):
        """Given a list and a size, return a rescaled/samples list. For example,
        if we want a list of size 5 and we have a list of size 25, return a new
        list of size five which is every 5th element of the origina list."""
        assert len(input_list) >= size

        # Get the number to skip between iterations.
        skip = len(input_list) // size

        # Build our new output.
        output = [input_list[i] for i in range(0, len(input_list), skip)]

        # Cut off the last one if needed.
        return output[:size]
    