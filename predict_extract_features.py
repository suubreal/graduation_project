"""
This script generates extracted features for each video, which other
models make use of.

You can change you sequence length and limit to a set number of classes
below.

class_limit is an integer that denotes the first N classes you want to
extract features from. This is useful is you don't want to wait to
extract all 101 classes. For instance, set class_limit = 8 to just
extract features for the first 8 (alphabetical) classes in the dataset.
Then set the same number when training models.
"""

import os
os.environ['TF_CPP_MIN_LOG_LEVEL']='2'

import numpy as np
import os.path
from predict_data import Predict_DataSet
from extractor import Extractor
from tqdm import tqdm

# Set defaults.
seq_length = 80
class_limit = None  # Number of classes to extract. Can be 1-101 or None for all.

print('before data')
# Get the dataset.
data = Predict_DataSet(seq_length=seq_length, class_limit=class_limit)

print('2')
# get the model.
#model = Extractor('data\\checkpoints\\inception.033-0.84.hdf5')
model = Extractor()

print('loop')
print(data.data)
# Loop through data.
#pbar = tqdm(total=len(data.data))
for video in data.data:
    print(video)
    
    # Get the path to the sequence for this video.
    path = '.\\data\\sequences\\' + video[1] + '-' + str(seq_length) + \
        '-features.txt'

    print('2')
    # Check if we already have it.
    if os.path.isfile(path):
        continue

    # Get the frames for this video.
    frames = data.get_frames_for_sample(video)
    
    #print(frames)

    # Now downsample to just the ones we need.
    frames = data.rescale_list(frames, seq_length)

    # Now loop through and extract features to build the sequence.
    sequence = []
    for image in frames:
        features = model.extract(image)
        sequence.append(features)

    # Save the sequence.
    np.savetxt(path, sequence)

    pbar.update(1)

pbar.close()
print('afterlooop')