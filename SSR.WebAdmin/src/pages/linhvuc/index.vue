<script>
import Layout from "../../layouts/main";

import PageHeader from "@/components/page-header";
import {required} from "vuelidate/lib/validators";
import appConfig from "@/app.config";
import {notifyModel} from "@/models/notifyModel";
import {pagingModel} from "@/models/pagingModel";
import {linhVucModel} from "@/models/linhVucModel";
import {CONSTANTS} from "@/helpers/constants";
export default {
  page: {
    title: "Lĩnh vực",
    meta: [{name: "description", content: appConfig.description}],
  },
  components: {Layout},
  data() {
    return {
      title: "Lĩnh vực",
      items: [
        {
          text: "Lĩnh vực",
          href: "/linh-vuc",
          // active: true,
        },
        {
          text: "Danh sách",
          active: true,
        }
      ],
      data: [],
      showModal: false,
      showDetail: false,
      showDeleteModal: false,
      submitted: false,
      model: linhVucModel.baseJson(),
      listCoQuan: [],
      listRole: [],
      pagination: pagingModel.baseJson(),
      totalRows: 1,
      todoTotalRows: 1,
      currentPage: 1,
      numberOfElement: 1,
      perPage: 10,
      pageOptions: [5,10, 25, 50, 100],
      filter: null,
      todoFilter: null,
      filterOn: [],
      todofilterOn: [],
      isBusy: false,
      sortBy: "age",
      sortDesc: false,
      fields: [
        { key: 'STT',
          label: 'STT',
          class: 'td-stt',
          sortable: false,
          thClass: 'hidden-sortable',
          thStyle: { width: '30px', minWidth: '30px' },
        },
        {
          key: "ten",
          label: "Tên",
          sortable: true,
          thStyle:"text-align:left",
        },
        {
          key: "thuTu",
          label: "Thứ tự",
          class: 'td-xuly',
          sortable: true,
          thStyle: { width: '100px', minWidth: '100px' },
        },
        {
          key: 'process',
          label: 'Xử lý',
          class: 'td-xuly btn-process',
          thClass: 'hidden-sortable',
          sortable: false,
          thStyle: { width: '120px', minWidth: '120px' },
        }
      ],
    };
  },
  validations: {
    model: {
      ten: {required},
      thuTu: {required}
    },
  },
  created() {
    this.fnGetList();
  },
  watch: {
    model: {
      deep: true,
      handler(val) {
        // addCoQuanToModel()
        // this.saveValueToLocalStorage()
      }
    },
    showModal(status) {
      if (status == false) this.model = linhVucModel.baseJson();
    },
    showDeleteModal(val) {
      if (val == false) {
        this.model.id = null;
      }
    },
  },
  methods: {
    async fnGetList() {
      await this.onPageChange();
    },
    async onPageChange(page = 1) {
      this.pagination.currentPage = page;
      const params = {
        pageNumber: this.pagination.currentPage,
        pageSize: this.pagination.pageSize,
      }
      this.$refs.tblList?.refresh()
    },
    async handleUpdate(id) {
      await this.$store.dispatch("dmLinhVucStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = linhVucModel.fromJson(res.data);
          this.showModal = true;
        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
          this.$refs.tblList.refresh()
        }
      });
    },
    async handleDetail(id) {
      await this.$store.dispatch("dmLinhVucStore/getById", id).then((res) => {
       if (res.resultCode === 'SUCCESS') {
          this.model = linhVucModel.fromJson(res.data);
          this.showDetail = true;
       } else {
         this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
      }
      });
    },
    async handleDelete() {
      if (this.model.id != 0 && this.model.id != null && this.model.id) {
          await this.$store.dispatch("dmLinhVucStore/delete", this.model.id).then((res) => {
            if (res.resultCode === 'SUCCESS') {
            this.fnGetList();
            this.showDeleteModal = false;
            this.$refs.tblList.refresh()
          }
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
          // });
        });
      }
    },
    handleShowDeleteModal(id) {
      this.model.id = id;
      this.showDeleteModal = true;
    },
    async handleSubmit(e) {
      e.preventDefault();
      this.submitted = true;
      this.$v.$touch();
      if (this.$v.$invalid) {
        return;
      } else {
        let loader = this.$loading.show({
          container: this.$refs.formContainer,
        });
        if (
            this.model.id != 0 &&
            this.model.id != null &&
            this.model.id
        ) {
          // Update model
          await this.$store.dispatch("dmLinhVucStore/update", this.model).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.fnGetList();
              this.showModal = false;
              this.$refs.tblList.refresh()
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
        } else {
          // Create model
          await this.$store.dispatch("dmLinhVucStore/create", linhVucModel.toJson(this.model)).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.fnGetList();
              this.showModal = false;
              this.$refs.tblList.refresh()
              this.model = {};
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
        }
        loader.hide();
      }
      this.submitted = false;
    },
    myProvider (ctx) {
      const params = {
        start: ctx.currentPage,
        limit: ctx.perPage,
        content: this.filter,
        sortBy: ctx.sortBy,
        sortDesc: ctx.sortDesc,
      }
      this.loading = true
      try {
        let promise =  this.$store.dispatch("dmLinhVucStore/getPagingParams", params)
        return promise.then(resp => {
          if(resp.resultCode == CONSTANTS.SUCCESS){
            let data = resp.data;
            this.totalRows = data.totalRows
            let items = data.data
            this.numberOfElement = items.length
            this.loading = false
            return items || []
          }else{
            return [];
          }
        })
      } finally {
        this.loading = false
      }
    },
    clearSearch(){
      this.filter = null;
    }
  }
}
</script>
<template>
  <Layout>
<!--    <PageHeader :title="title" :items="items"/>-->
    <div class="row">
      <div class="col-12">
        <div class="card mb-2">
          <div class="card-body">
            <div class="row">
              <div class="col-md-4 col-12 d-flex align-items-center">
                <h4 class="font-size-18 fw-bold text-dark">Lĩnh vực</h4>
              </div>
              <div class="col-md-8 col-12 text-end">
                <b-button v-b-toggle.collapseSearch variant="light"
                          class="btn w-md btn-primary-outline me-2" size="sm">
                  <i class="fas fa-caret-down align-middle me-2"></i>
                  Tìm kiếm
                </b-button>
                <b-button
                    variant="primary"
                    type="button"
                    class="btn w-md btn-primary"
                    @click="showModal = true"
                    size="sm"
                >
                  <i class="mdi mdi-plus me-1"></i> Tạo lĩnh vực
                </b-button>
              </div>
            </div>
            <b-collapse id="collapseSearch" class="mt-1">
              <div class="row">
                <div class="col-12">
                  <div class="d-flex justify-content-between align-items-end flex-wrap mb-2">
                    <!-- Nội dung -->
                    <div class="flex-grow-1 me-2">
                      <label>Nội dung</label>
                      <input
                          v-model = "filter"
                          type="text"
                          class="form-control"
                          placeholder="Tìm kiếm ..."
                      />
                      <i class="bx bx-search-alt search-icon"></i>
                    </div>
                    <div class="flex-grow-0 ms-2">
                      <div class="d-flex justify-content-between align-items-center flex-wrap">
                        <div class="flex-grow-1 mt-xl-0 mt-2">
                          <b-button @click="handleSearch" variant="light"
                                    class="btn w-md btn-primary me-2" size="md">
                            <i class="fas fa-search align-middle me-2"></i>
                            Tìm kiếm
                          </b-button>
                        </div>
                        <div class="flex-grow-1 mt-xl-0 mt-2">
                          <b-button @click="clearSearch" variant="light"
                                    class="btn w-md btn-secondary me-2" size="md">
                            <i class="fas fa-redo-alt align-middle me-2"></i>
                            Làm mới
                          </b-button>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </b-collapse>
          </div>
        </div>
      </div>
    </div>
    <div class="row">
      <div class="col-12">
        <div class="card">
          <div class="card-body">
            <div class="row mb-2">
              <div class="col-sm-4">
                <div class="search-box me-2 mb-2 d-inline-block">
                  <div class="position-relative">

                  </div>
                </div>
              </div>
              <div class="col-sm-8">
                <div class="text-sm-end">
                  <b-modal
                      v-model="showModal"
                      title="Thông tin lĩnh vực"
                      title-class="text-black font-18"
                      body-class="p-3"
                      hide-footer
                      centered
                      no-close-on-backdrop
                      size="lg"
                  >
                    <form @submit.prevent="handleSubmit"
                          ref="formContainer">
                      <div class="row">
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Tên lĩnh vực</label>
                            <span style="color: red">&nbsp;*</span>
                            <input type="hidden" v-model="model.id"/>
                            <input
                                id="ten"
                                v-model.trim="model.ten"
                                type="text"
                                class="form-control"
                                placeholder="Nhập tên lĩnh vực"
                                :class="{
                                'is-invalid':
                                  submitted && $v.model.ten.$error,
                              }"
                            />
                            <div
                                v-if="submitted && !$v.model.ten.required"
                                class="invalid-feedback"
                            >
                              Tên lĩnh vực không được để trống.
                            </div>
                          </div>
                        </div>
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Nhập số thứ tự</label>
                            <span style="color: red">&nbsp;*</span>
                            <input type="hidden" v-model="model.id"/>
                            <input
                                id="thuTu"
                                v-model="model.thuTu"
                                type="number"
                                min="0"
                                oninput="validity.valid||(value='');"
                                class="form-control"
                                placeholder="Nhập số thứ tự"
                                :class="{
                                'is-invalid':
                                  submitted && $v.model.thuTu.$error,
                              }"
                            />
                            <div
                                v-if="submitted && !$v.model.thuTu.required"
                                class="invalid-feedback"
                            >
                              Thứ tự không được để trống.
                            </div>
                          </div>
                        </div>
                      </div>
                      <div class="text-end pt-2 mt-3">
                        <b-button variant="light" class="w-md" size="sm" @click="showModal = false">
                          Đóng
                        </b-button>
                        <b-button type="submit" variant="primary" size="sm" class="ms-1 w-md">
                          Lưu
                        </b-button>
                      </div>
                    </form>
                  </b-modal>
                  <b-modal
                      v-model="showDetail"
                      title="Thông tin chi tiết lĩnh vực"
                      title-class="text-black font-18"
                      body-class="p-3"
                      hide-footer
                      centered
                      no-close-on-backdrop
                      size="lg"
                  >
                    <form @submit.prevent="handleSubmit"
                          ref="formContainer">
                      <div class="row">
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Tên lĩnh vực : </label>
                            <input
                                v-model="model.ten"
                                type="text"
                                class="form-control"
                            />
                          </div>
                        </div>
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Thứ tự : </label>
                            <input
                                v-model="model.thuTu"
                                type="number"
                                min="0"
                                oninput="validity.valid||(value='');"
                                class="form-control"
                            />
                          </div>
                        </div>
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Người tạo : </label>
                            <input
                                v-model="model.createdBy"
                                type="text"
                                class="form-control"
                            />
                          </div>
                        </div>
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Ngày tạo: </label>
                            <input
                                v-model="model.createdAtShow"
                                type="text"
                                class="form-control"
                            />
                          </div>
                        </div>
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Người cập nhật : </label>
                            <input
                                v-model="model.modifiedBy"
                                type="text"
                                class="form-control"
                            />
                          </div>
                        </div>
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Ngày cập nhật : </label>
                            <input
                                v-model="model.lastModifiedShow"
                                type="text"
                                class="form-control"
                            />
                          </div>
                        </div>
                      </div>
                      <div class="text-end pt-2 mt-3">
                        <b-button variant="light" @click="showDetail = false">
                          Đóng
                        </b-button>
                      </div>
                    </form>
                  </b-modal>
                </div>
              </div>
            </div>
            <div class="row">
              <div class="col-12">
                    <div class="row mb-3">
                      <div class="col-sm-12 col-md-6">
                        <div id="tickets-table_length" class="dataTables_length">
                          <label class="d-inline-flex align-items-center">
                             Hiện
                            <b-form-select
                                class="form-select form-select-sm"
                                v-model="perPage"
                                size="sm"
                                :options="pageOptions"
                            ></b-form-select
                            >&nbsp;dòng
                          </label>
                        </div>
                      </div>
                    </div>
                    <div class="table-responsive-sm">
                      <b-table
                          class="datatables custom-table"
                          :items="myProvider"
                          :fields="fields"
                          striped
                          bordered
                          responsive="sm"
                          :per-page="perPage"
                          :current-page="currentPage"
                          :sort-by.sync="sortBy"
                          :sort-desc.sync="sortDesc"
                          :filter="filter"
                          :filter-included-fields="filterOn"
                          ref="tblList"
                          primary-key="id"
                          :busy.sync="isBusy"
                          tbody-tr-class="b-table-linhvuc"
                      >
                        <template v-slot:cell(STT)="data">
                          {{ data.index + ((currentPage-1)*perPage) + 1  }}
                        </template>
                        <template v-slot:cell(process)="data">
                          <button
                              type="button"
                              size="sm"
                              class="btn btn-detail btn-sm"
                              data-toggle="tooltip" data-placement="bottom" title="Chi tiết"
                              v-on:click="handleDetail(data.item.id)">
                            <i class="fas fa-eye "></i>
                          </button>
                          <button
                              type="button"
                              size="sm"
                              class="btn btn-edit btn-sm"
                              data-toggle="tooltip" data-placement="bottom" title="Cập nhật"
                              v-on:click="handleUpdate(data.item.id)">
                            <i class="fas fa-pencil-alt"></i>
                          </button>
                          <button
                              type="button"
                              size="sm"
                              class="btn btn-delete btn-sm"
                              data-toggle="tooltip" data-placement="bottom" title="Xóa"
                              v-on:click="handleShowDeleteModal(data.item.id)">
                            <i class="fas fa-trash-alt"></i>
                          </button>
                        </template>
                        <template v-slot:cell(ten)="data">&nbsp;&nbsp;
                          {{data.item.ten}}
                        </template>
                      </b-table>
                      <template v-if="isBusy">
                        <div align="center">Đang tải dữ liệu</div>
                      </template>
                      <template v-if="totalRows <= 0 && !isBusy">
                        <div align="center">Không có dữ liệu</div>
                      </template>
                    </div>
                    <div class="row">
                      <b-col>
                        <div>Hiển thị {{numberOfElement}} trên tổng số {{totalRows}} dòng</div>
                      </b-col>
                      <div class="col">
                        <div
                            class="dataTables_paginate paging_simple_numbers float-end">
                          <ul class="pagination pagination-rounded mb-0">
                             <!-- pagination -->
                            <b-pagination
                                v-model="currentPage"
                                :total-rows="totalRows"
                                :per-page="perPage"
                            ></b-pagination>
                          </ul>
                        </div>
                      </div>
                    </div>

                  </div>
                </div>
              </div>
            </div>
            <b-modal
                v-model="showDeleteModal"
                centered
                title="Xóa dữ liệu"
                title-class="font-18"
                no-close-on-backdrop
            >
              <p>
                Dữ liệu xóa sẽ không được phục hồi!
              </p>
              <template #modal-footer>
                <b-button v-b-modal.modal-close_visit
                          size="sm"
                        class="btn btn-outline-info w-md"
                        v-on:click="showDeleteModal = false">
                  Đóng
                </b-button>
                <b-button v-b-modal.modal-close_visit
                        size="sm"
                        variant="danger"
                        type="button"
                        class="w-md"
                        v-on:click="handleDelete">
                  Xóa
                </b-button>
              </template>
            </b-modal>
          </div>
        </div>
  </Layout>
</template>
<style>
.td-stt {
  text-align: center;
}
.td-xuly {
  text-align: center;
}
.table>tbody> tr >td{
  padding: 0px;
  line-height: 30px;
}
.hidden-sortable:after, .hidden-sortable:before {
  display: none !important;
}
@media only screen and (max-width: 768px) {
  .b-table-linhvuc>td:nth-of-type(1):before{
    content: "STT";
    font-weight: bold;
    color: #00568c;
  }
  .b-table-linhvuc>td:nth-of-type(2):before {
    content: "Tên";
    font-weight: bold;
    color: #00568c;
  }
  .b-table-linhvuc>td:nth-of-type(3):before {
    content: "Thứ tự";
    font-weight: bold;
    color: #00568c;
  }
  .b-table-linhvuc>td:nth-of-type(4):before {
    content: "";
    font-weight: bold;
    color: #00568c;
  }
}

</style>
