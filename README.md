# password manager

this is a simple password manager built in c# that allows users to securely store and retrieve passwords. the program encrypts passwords using aes encryption, and each password is saved with a note for easy reference. the encryption key is user-defined and can be changed at any time, with different profiles to store separate password data.

## features

- **create password**: generate a random password, encrypt it, and store it with a note.
- **view saved passwords**: view the list of stored passwords (decrypted).
- **switch profile**: switch between different profiles and encryption keys.
- **secure encryption**: passwords are encrypted using aes with a user-provided key.
- **profile-based storage**: each profile has its own encrypted password file.

## how to use

1. **start the program**: when the program starts, it will ask for an encryption key and profile name.
2. **choose an action**:
    - `1. create password`: generate a new password, encrypt it, and save it with a note.
    - `2. view saved passwords`: view your saved passwords (decrypted).
    - `3. switch profile`: switch to a different profile and encryption key.
    - `0. exit`: exit the program.
3. **changing profiles**: you can switch to a new profile at any time by choosing the "switch profile" option. this will prompt for a new encryption key and profile name.
4. **password storage**: passwords are stored in `data` folder in the root directory with the name `[profile name]_encrypted.sec`.

## encryption details

the encryption uses **aes** (advanced encryption standard) with the following characteristics:
- encryption key: generated from a user-provided passphrase using sha256.
- initialization vector (iv): generated randomly for each password encryption.
- the password file is saved with the iv prepended to the encrypted password data.

## how it works

### 1. user input for encryption key and profile
- on startup, the user is asked to input a custom encryption key.
- the key is processed (hashed with sha256) to ensure proper length for aes encryption.
- the user is also asked to input a profile name, which is used to determine the file path for storing passwords.

### 2. creating passwords
- a random password of 16 characters is generated.
- the password is then encrypted using the processed encryption key.
- the encrypted password is stored in a json file under the user's chosen profile name.

### 3. viewing saved passwords
- the program reads the saved password file, decrypts the stored passwords, and displays them to the user.
- if the decryption fails, the program returns `?` for each character of the failed password.

### 4. switching profiles
- you can change your encryption key and profile at any time by selecting the "switch profile" option.
- this will reset the password file path and prompt you for a new key and profile name.

