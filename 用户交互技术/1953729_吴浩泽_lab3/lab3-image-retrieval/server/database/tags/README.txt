MIRFLICKR-25k Annotations v0.80, 3 July 2009

This version adds the relevant labels for the following categories:
people, baby, female, male and portrait.

The following summarizes the current state of the annotations:

== CONTENTS
===========

The annotations are for the complete MIRFLICKR-25000 collection.  Please refer
to [1] for benchmarks as well as suggestions for divisions in training and test
sets.

The topic/concept annotations included in this version are:

= POTENTIAL LABELS

sky, clouds, water, sea, river, lake, people, portrait,
male, female, baby, night, plantlife, tree, flower, animals,
dog, bird, structures, sunset, indoor, transport, car

These are in the text files: sky.txt, clouds.txt etc.

Note that these potential labels are intended to make it easier to obtain
'real' annotations (i.e. relevant labels), but they may be interesting for
classification in their own right (see also [1] and
http://press.liacs.nl/mirflickr/.)

= RELEVANT LABELS:

clouds, sea, lake, river, night, tree, flower, dog, bird, car, people,
baby, female, male, portrait

These annotations are in the text files clouds_r1.txt, sea_r1.txt etc.

== FORMAT
=========

The annotation files are plain text files. The file contains the numbers
(one per line) of the images labeled with the concept/topic.

Note that an image often appears in several annotation files: in that
case more than one label applies to the image.

== IN PROGRESS
==============

The following annotations are still underway:

= POTENTIAL LABELS

the sub-topics of (man-made) structures (architecture, building, house, 
city/urban, bridge, road/street).

= RELEVANT LABELS

annotations for missing general topics (sky, water etc) and the subtopics
of 'structures'

== FUTURE
=========

The relevant labels supplied so far are part of the first round of annotations.
The goal of our annotation setup is to obtain several personal interpretations 
of the topics. For the first interpretations supplied here, we have aimed to label 
only images for which the 'topic is saliently present according to the annotator'. 
Additional annotations will be obtained by repeating this process with different
annotators, as well as by targeting different interpretations (e.g. for 'birds' we may
consider the interpretation 'small singing birds'). The additional annotations
will be made available in files birds_r2.txt, birds_r3.txt etc.

An important reason for this approach is that we want to make the annotations
suitable for relevance feedback benchmarking, see [2].

Extending the annotations in this direction will be our first priority; next, we
will consider additional topics.

When new annotations become available all users who registered with their email
address will be notified.

== FEEDBACK
===========

Please send feedback or suggestions for improvement to mark.huiskes@liacs.nl.


== REFERENCES
=============

[1] M. J. Huiskes, M. S. Lew (2008). The MIR Flickr Retrieval Evaluation. ACM
International Conference on Multimedia Information Retrieval (MIR'08),
Vancouver, Canada.

[2] M.J. Huiskes, M.S. Lew (2008). ``Performance Evaluation of Relevance Feedback 
Methods'', ACM International Conference on Video and Image Retrieval (CIVR'08), 
p. 239--248, Niagara Falls, Canada.

