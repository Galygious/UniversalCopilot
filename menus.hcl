#menu.hcl
menu "MainMenu" {
	submenu "Summaries" {
		action "Summarize" {
			requires_user_input = true
			user_input_prompt = "Enter desired length of summary"
			vars = {
				var1 = "selectedText"
				var2 = "userInput"
			}
			prompt = "Summarize the following text in {var2}:\n{var1}"
			prompt_if_no_selected_text = "No selected text found. Please enter text:"
			fallback_var = "var1"
		}

		action "Fill" {
			vars = {
				var1 = "selectedText"
			}
			prompt = "Give the most concise possible answer...\n```{var1}```"
			prompt_if_no_selected_text = "No selected text found. Please enter text:"
			fallback_var = "var1"
		}

		action "Custom Summarize" {
			requires_user_input = true
			user_input_prompt = "Enter custom instructions:"
			vars = {
				var1 = "selectedText"
				var2 = "userInput"
			}
			prompt = "{var2}\nNow summarize:\n{var1}"
			prompt_if_no_selected_text = "No selected text found. Please enter text:"
			fallback_var = "var1"
		}
	}
	submenu "Translate"{
		action "Spanish" {
			vars = {
				var1 = "selectedText"
			}
			prompt = "Translate the following text to spanish:\n{var1}"
			prompt_if_no_selected_text = "No selected text found. Please enter text:"
			fallback_var = "var1"
		}

		action "Custom Translate" {
			requires_user_input = true
			user_input_prompt = "Enter a custom language name:"
			vars = {
				var1 = "selectedText"
				var2 = "userInput"
			}
			prompt = "Translate the following text to {var2}:\n{var1}"
			prompt_if_no_selected_text = "No selected text found. Please enter text:"
			fallback_var = "var1"
		}
	}
	submenu "Story"{
		action "Dog"{
			prompt = "Write a story about a dog"
		}
	}
	action "Exit" {
	}
}