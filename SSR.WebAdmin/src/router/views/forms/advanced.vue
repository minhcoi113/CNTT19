<script>
import DatePicker from "vue2-datepicker";
import Switches from "vue-switches";
import Multiselect from "vue-multiselect";

import Layout from "../../layouts/main";
import PageHeader from "@/components/page-header";
import appConfig from "@/app.config";

/**
 * Form Advanced component
 */
export default {
  page: {
    title: "Form Advanced",
    meta: [{ name: "description", content: appConfig.description }]
  },
  components: { DatePicker, Switches, Multiselect, Layout, PageHeader },
  data() {
    return {
      title: "Form Advanced",
      items: [
        {
          text: "Veltrix",
          href: "/"
        },
        {
          text: "Forms",
          href: "/"
        },
        {
          text: "Form Advanced",
          active: true
        }
      ],
      defaultdate: "",
      autoClose: "",
      daterange: "",
      demoMonth: "",
      demoYear: "",
      time: "",
      enabled: false,
      enabled1: false,
      enabled2: false,
      enabled3: false,
      enabled4: false,
      enabled5: false,
      small: false,
      small1: false,
      small2: false,
      small3: false,
      danger: false,
      info: false,
      success: false,
      primary: false,
      secondary: false,
      customColor: false,
      customColor1: false,
      customColor2: false,
      customColor3: false,
      value: null,
      value1: null,
      options: [
        "Alaska",
        "Hawaii",
        "California",
        "Nevada",
        "Oregon",
        "Washington",
        "Arizona",
        "Colorado",
        "Idaho",
        "Montana",
        "Nebraska",
        "New Mexico",
        "North Dakota",
        "Utah",
        "Wyoming",
        "Alabama",
        "Arkansas",
        "Illinois",
        "Iowa"
      ],
      maxlen: "",
      data: "",
      option: "",
      textarea: ""
    };
  }
};
</script>

<template>
  <Layout>
    <PageHeader :title="title" :items="items" />
    <div class="row">
      <div class="col-lg-6">
        <div class="card">
          <div class="card-body">
            <h4 class="mb-4 card-title">Select2</h4>
            <label class="form-label">Single Select</label>

            <multiselect v-model="value" :options="options"></multiselect>
            <label class="form-label mt-3">Multiple Select</label>

            <multiselect v-model="value1" :options="options" :multiple="true"></multiselect>
          </div>
        </div>
      </div>
      <!-- end col -->
      <div class="col-lg-6">
        <div class="card">
          <div class="card-body">
            <h4 class="mb-3 card-title">Switchery</h4>
            <h6>Basic</h6>
            <p class="text-muted mb-3 font-13">
              Add an attribute
              <code>data-plugin="switchery" data-color="@colors"</code>
              to your input element and it will be converted into switch.
            </p>
            <switches v-model="enabled" type-bold="true" class="mb-0"></switches>
            <switches v-model="enabled1" type-bold="false" color="warning" class="ml-1 mb-0"></switches>
            <switches v-model="enabled2" type-bold="false" color="danger" class="ml-1 mb-0"></switches>
            <switches v-model="enabled3" type-bold="false" color="primary" class="ml-1 mb-0"></switches>
            <switches v-model="enabled4" type-bold="false" color="success" class="ml-1 mb-0"></switches>
            <switches v-model="enabled5" type-bold="false" color="info" class="ml-1 mb-0"></switches>
            <h6>Sizes</h6>
            <p class="text-muted mb-3 font-13">
              Add an attribute
              <code>data-size="small"</code>
              to your input element and it will be converted into switch.
            </p>
            <div>
              <switches v-model="small"></switches>
              <switches v-model="small1" color="warning" class="ml-1"></switches>
              <switches v-model="small2" color="danger" class="ml-1"></switches>
              <switches v-model="small3" color="success" class="ml-1"></switches>
            </div>
          </div>
        </div>
      </div>
      <!-- end col -->
    </div>
    <!-- end row -->
    <div class="row">
      <div class="col-lg-6">
        <div class="card">
          <div class="card-body">
            <h4 class="card-title">Date Picker</h4>
            <p class="card-title-desc">Examples of bootstrap datepicker.</p>

            <div>
              <div class="mb-3">
                <label class="form-label">Default Date Picker</label>
                <br />
                <date-picker
                  v-model="defaultdate"
                  :first-day-of-week="1"
                  format="MM/DD/YYYY"
                  lang="en"
                  confirm
                  placeholder="Select date"
                ></date-picker>
              </div>

              <div class="mb-3">
                <label class="form-label">Auto close</label>
                <br />
                <date-picker
                  v-model="autoClose"
                  format="MM/DD/YYYY"
                  :first-day-of-week="1"
                  lang="en"
                  placeholder="Select date"
                ></date-picker>
              </div>

              <div class="mb-3">
                <label class="form-label">Date Range</label>
                <br />
                <date-picker
                  v-model="daterange"
                  format="MM/DD/YYYY"
                  range
                  append-to-body
                  lang="en"
                  confirm
                  placeholder="Select date"
                ></date-picker>
              </div>

              <div class="mb-3">
                <label class="form-label">Month View</label>
                <br />
                <date-picker
                  v-model="demoMonth"
                  type="month"
                  lang="en"
                  confirm
                  placeholder="Select Month"
                ></date-picker>
              </div>

              <div class="mb-3">
                <label class="form-label">Year View</label>
                <br />
                <date-picker
                  v-model="demoYear"
                  type="year"
                  lang="en"
                  confirm
                  placeholder="Select Year"
                ></date-picker>
              </div>

              <div class="mb-3">
                <label class="form-label">Time View</label>
                <br />
                <date-picker
                  v-model="time"
                  format="hh:mm:ss a"
                  value-type="format"
                  type="time"
                  placeholder="hh:mm:ss a"
                ></date-picker>
              </div>
            </div>
          </div>
        </div>
        <!-- End datepicker -->
      </div>
      <!-- end col -->
      
      <div class="col-lg-6">
        <div class="card">
          <div class="card-body">
            <h4 class="card-title">Bootstrap MaxLength</h4>
            <p class="card-title-desc">Uses the HTML5 attribute "maxlength" to work.</p>

            <label class="mb-1 font-weight-medium">Default usage</label>
            <p
              class="text-muted font-13"
            >The badge will show up by default when the remaining chars are 10 or less:</p>
            <input v-model="maxlen" type="text" class="form-control" :maxlength="10" />
            <div v-if="maxlen" class="text-center">
              <p
                class="badge position-absolute"
                :class="{ 'bg-success': maxlen.length !== 10,
                            'bg-danger': maxlen.length === 10 }"
              >{{ maxlen.length }} / 10</p>
            </div>

            <div>
              <label class="mb-1 mt-3 font-weight-medium">Threshold value</label>
              <p class="text-muted font-13">
                Do you want the badge to show up when there are 20 chars or less? Use the
                <code>threshold</code>
                option:
              </p>
              <input v-model="data" type="text" :maxlength="25" class="form-control" />
              <div class="text-center">
                <p
                  v-if="data"
                  class="badge position-absolute"
                  :class="{ 'bg-success': data.length !== 25,
                            'bg-danger': data.length === 25 }"
                >{{ data.length }} / 25</p>
              </div>
            </div>

            <div>
              <label class="mb-1 mt-3 font-weight-medium">All the options</label>
              <p class="text-muted font-13">
                Please note: if the
                <code>alwaysShow</code> option is enabled, the
                <code>threshold</code> option
                is
                ignored.
              </p>
              <input v-model="option" type="text" class="form-control" :maxlength="25" />
              <div class="text-center">
                <p
                  v-if="option"
                  class="badge position-absolute"
                  :class="{ 'bg-success': option.length !== 25,
                            'bg-danger': option.length === 25 }"
                >
                  You typed
                  {{ option.length }} out of 25 chars available.
                </p>
              </div>
            </div>

            <div>
              <label class="mb-1 mt-3 font-weight-medium">Textareas</label>
              <p
                class="text-muted font-13"
              >Bootstrap maxlength supports textarea as well as inputs. Even on old IE.</p>
              <textarea
                v-model="textarea"
                class="form-control"
                :maxlength="225"
                rows="3"
                placeholder="This textarea has a limit of 225 chars."
              ></textarea>
              <div class="text-center">
                <p
                  v-if="textarea"
                  class="badge position-absolute"
                  :class="{ 'bg-success': textarea.length !== 225,
                            'bg-danger': textarea.length === 225 }"
                >
                  {{ textarea.length }} /
                  225
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
      <!-- end col -->
    </div>
    <!-- end row -->
  </Layout>
</template>