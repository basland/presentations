import os
from setuptools import setup, find_packages

current_directory = os.path.abspath(os.path.dirname(__file__))
version_file_path = os.path.join(current_directory, 'version.txt')

with open(version_file_path, 'r') as version_file:
    version = version_file.read().strip()
 
with open("README.md", "r") as fh:
    long_description = fh.read()

setup(
    name='upsert-demo',
    version=version,
    author="Kimura Data Intelligence B.V., The Netherlands",
    author_email='info@kimura.nl',
    description="A demo.",
    long_description=long_description,
    long_description_content_type='text/markdown',
    packages=find_packages(),
    classifiers=[
        "Programming Language :: Python :: 3",
        "License :: Other/Proprietary License",
        "Operating System :: OS Independent",
    ],
    python_requires='>=3.7',
)