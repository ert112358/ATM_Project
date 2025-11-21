import { Image } from "expo-image";
import { ThemedView } from "@/components/themed-view";
import { ThemedText } from "@/components/themed-text";
import ParallaxScrollView from "@/components/parallax-scroll-view";
import { Fonts } from "@/constants/theme";
import { useState, useEffect } from "react";
import { Alert } from "react-native";

import {
  View,
  TextInput,
  TouchableOpacity,
  Text,
  StyleSheet,
} from "react-native";

export default function HomeScreen() {
  const passwordRegex = new RegExp(
    "^(?=.*\\d)(?=.*[A-Z])(?=.*[a-z])(?=.*[^\\w\\d\\s:])([^\\s]){8,16}$",
  );
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [errors, setErrors] = useState({});
  const [isFormValid, setIsFormValid] = useState(false);

  useEffect(() => {
    // Trigger form validation when name,
    // email, or password changes
    validateForm();
  }, [username, password]);

  const validateForm = () => {
    let errors = {};

    // Validate name field
    if (!username) {
      errors.name = "Name is required.";
    }

    // Validate password field
    if (!password) {
      errors.password = "Password is required.";
    } else if (!passwordRegex.test(password)) {
      errors.password =
        "The password must contain at least: \n\
 * 1 number (0-9)\n\
 * 1 uppercase letter\n\
 * 1 lowercase letter\n\
 * 1 non-alpha numeric number\n\
 * and be 8-16 characters long\n    with no spaces.";
    }

    // Set the errors and update form validity
    setErrors(errors);
    setIsFormValid(Object.keys(errors).length === 0);
  };

  const handleSubmit = async () => {
    validateForm();

    if (!isFormValid) {
      console.log("Form is not valid.");
      return;
    }

    const loginAPI = await fetch(
      "http://zeropage.it:5001/api/register?username=" +
        username +
        "&password=" +
        password,
    );

    const status = loginAPI.status;

    if (status == 200) {
      Alert.alert(
        "Success",
        "You've been successfully registered to the platform. You may now login.",
        [{ text: "OK" }],
      );
    } else if (status == 400) {
      // Bad Request

      const response = await loginAPI.json();

      if (response === "Username already taken") {
        Alert.alert(
          "Failed",
          "This username is already registered. Please try another one.",
          [{ text: "OK" }],
        );
      } else {
        Alert.alert(
          "Error",
          "An unknown error has occurred. Please try again.",
          [{ text: "OK" }],
        );
      }
    }

    //const response = await loginAPI.json();
  };

  return (
    <ParallaxScrollView
      headerBackgroundColor={{ light: "#A1CEDC", dark: "#1D3D47" }}
      headerImage={
        <Image
          source={require("@/assets/images/atm_pic.jpg")}
          style={{ width: "100%", height: "100%" }}
        />
      }
    >
      <ThemedView style={styles.titleContainer}>
        <ThemedText
          type="title"
          style={{
            fontFamily: Fonts.rounded,
          }}
        >
          Register
        </ThemedText>
      </ThemedView>
      <View style={styles.container}>
        <TextInput
          style={styles.input}
          placeholder="Name"
          value={username}
          onChangeText={setUsername}
        />
        <TextInput
          style={styles.input}
          placeholder="Password"
          value={password}
          onChangeText={setPassword}
          secureTextEntry
        />
        <TouchableOpacity
          style={[styles.button, { opacity: isFormValid ? 1 : 0.5 }]}
          //disabled={!isFormValid}
          onPress={handleSubmit}
        >
          <Text style={styles.buttonText}>Submit</Text>
        </TouchableOpacity>

        {/* Display error messages */}
        {Object.values(errors).map((error, index) => (
          <Text key={index} style={styles.error}>
            {error}
          </Text>
        ))}
      </View>
    </ParallaxScrollView>
  );
}

const styles = StyleSheet.create({
  titleContainer: {
    flexDirection: "row",
    alignItems: "center",
    gap: 8,
  },
  stepContainer: {
    gap: 8,
    marginBottom: 8,
  },
  reactLogo: {
    height: 459,
    width: 612,
    bottom: 0,
    left: 0,
    position: "absolute",
  },
  container: {
    flex: 1,
    padding: 16,
    justifyContent: "center",
  },
  input: {
    height: 60,
    color: "black",
    borderColor: "#ccc",
    borderWidth: 1,
    marginBottom: 12,
    paddingHorizontal: 10,
    borderRadius: 8,
    fontSize: 16,
  },
  button: {
    backgroundColor: "green",
    borderRadius: 8,
    paddingVertical: 10,
    alignItems: "center",
    marginTop: 16,
    marginBottom: 12,
  },
  buttonText: {
    color: "#fff",
    fontWeight: "bold",
    fontSize: 16,
  },
  error: {
    color: "red",
    fontSize: 20,
    marginBottom: 12,
  },
});
